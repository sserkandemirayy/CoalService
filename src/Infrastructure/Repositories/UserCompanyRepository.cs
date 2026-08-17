using System.Data;
using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;

public class UserCompanyRepository : IUserCompanyRepository
{
    private const long MaximumUserSequence = 9_999_999;

    private readonly AppDbContext _db;
    private readonly ICurrentUserService _currentUser;

    public UserCompanyRepository(
        AppDbContext db,
        ICurrentUserService currentUser)
    {
        _db = db;
        _currentUser = currentUser;
    }

    public async Task<string?> AddOrReactivateAsync(
        Guid userId,
        Guid companyId,
        string? requestedUserCode = null,
        CancellationToken ct = default)
    {
        if (!CanAccessCompany(companyId))
            return null;

        var existing = await _db.UserCompanies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.CompanyId == companyId,
                ct);

        // ---------------------------------------------------------
        // Kullanýcý bu firmaya daha önce atanmýþ
        // ---------------------------------------------------------
        if (existing is not null)
        {
            // Frontend manuel kod gönderdiyse o kod kullanýlacak.
            if (!string.IsNullOrWhiteSpace(requestedUserCode))
            {
                var normalizedCode =
                    UserCompany.NormalizeUserCode(requestedUserCode);

                var codeExists = await UserCodeExistsAsync(
                    companyId,
                    normalizedCode,
                    userId,
                    ct);

                if (codeExists)
                {
                    throw new InvalidOperationException(
                        $"User code '{normalizedCode}' already exists in this company.");
                }

                existing.SetUserCode(normalizedCode);

                await EnsureCounterForManualSystemCodeAsync(
                    companyId,
                    normalizedCode,
                    ct);
            }
            else if (string.IsNullOrWhiteSpace(existing.UserCode))
            {
                var generatedCode =
                    await GenerateAutomaticUserCodeAsync(
                        companyId,
                        ct);

                existing.SetUserCode(generatedCode);
            }

            // Soft deleted ise tekrar aktifleþtir.
            if (existing.DeletedAt is not null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy =
                    _currentUser.GetCurrentUserId();
            }

            _db.UserCompanies.Update(existing);

            return existing.UserCode;
        }

        // ---------------------------------------------------------
        // Yeni UserCompany
        // ---------------------------------------------------------

        string userCode;

        if (!string.IsNullOrWhiteSpace(requestedUserCode))
        {
            userCode =
                UserCompany.NormalizeUserCode(
                    requestedUserCode);

            var codeExists = await UserCodeExistsAsync(
                companyId,
                userCode,
                null,
                ct);

            if (codeExists)
            {
                throw new InvalidOperationException(
                    $"User code '{userCode}' already exists in this company.");
            }

            await EnsureCounterForManualSystemCodeAsync(
                companyId,
                userCode,
                ct);
        }
        else
        {
            userCode =
                await GenerateAutomaticUserCodeAsync(
                    companyId,
                    ct);
        }

        var userCompany = UserCompany.Create(
            userId,
            companyId,
            userCode);

        await _db.UserCompanies.AddAsync(
            userCompany,
            ct);

        return userCode;
    }

    public async Task<bool> SetUserCodeAsync(
        Guid userId,
        Guid companyId,
        string userCode,
        CancellationToken ct = default)
    {
        if (!CanAccessCompany(companyId))
            return false;

        var normalizedCode =
            UserCompany.NormalizeUserCode(userCode);

        var relation = await _db.UserCompanies
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.CompanyId == companyId,
                ct);

        if (relation is null)
            return false;

        var exists = await UserCodeExistsAsync(
            companyId,
            normalizedCode,
            userId,
            ct);

        if (exists)
        {
            throw new InvalidOperationException(
                $"User code '{normalizedCode}' already exists in this company.");
        }

        relation.SetUserCode(normalizedCode);
        relation.UpdatedAt = DateTime.UtcNow;
        relation.UpdatedBy =
            _currentUser.GetCurrentUserId();

        await EnsureCounterForManualSystemCodeAsync(
            companyId,
            normalizedCode,
            ct);

        _db.UserCompanies.Update(relation);

        return true;
    }

    public async Task<bool> UserCodeExistsAsync(
        Guid companyId,
        string userCode,
        Guid? excludeUserId = null,
        CancellationToken ct = default)
    {
        var normalizedCode =
            UserCompany.NormalizeUserCode(userCode);

        var query = _db.UserCompanies
            .IgnoreQueryFilters()
            .Where(x =>
                x.CompanyId == companyId &&
                x.UserCode == normalizedCode);

        if (excludeUserId.HasValue)
        {
            query = query.Where(
                x => x.UserId != excludeUserId.Value);
        }

        return await query.AnyAsync(ct);
    }

    public async Task RemoveAsync(
        Guid userId,
        Guid companyId,
        Guid performedBy,
        CancellationToken ct = default)
    {
        if (!CanAccessCompany(companyId))
            return;

        var link = await _db.UserCompanies
            .FirstOrDefaultAsync(
                x => x.UserId == userId &&
                     x.CompanyId == companyId,
                ct);

        if (link is null)
            return;

        link.DeletedAt = DateTime.UtcNow;
        link.DeletedBy = performedBy;
        link.UpdatedAt = DateTime.UtcNow;
        link.UpdatedBy = performedBy;

        _db.UserCompanies.Update(link);

        var branchIds = await _db.Branches
            .Where(b => b.CompanyId == companyId)
            .Select(b => b.Id)
            .ToListAsync(ct);

        var userBranches = await _db.UserBranches
            .Where(ub =>
                ub.UserId == userId &&
                branchIds.Contains(ub.BranchId) &&
                ub.DeletedAt == null)
            .ToListAsync(ct);

        foreach (var userBranch in userBranches)
        {
            userBranch.DeletedAt = DateTime.UtcNow;
            userBranch.DeletedBy = performedBy;
            userBranch.UpdatedAt = DateTime.UtcNow;
            userBranch.UpdatedBy = performedBy;
        }

        _db.UserBranches.UpdateRange(userBranches);
    }

    public async Task<IEnumerable<Company>>
        GetCompaniesByUserIdAsync(
            Guid userId,
            CancellationToken ct = default)
    {
        var companyIds =
            _currentUser.GetCurrentUserCompanyIds();

        var unrestricted =
            HasUnrestrictedScope();

        return await _db.UserCompanies
            .Where(x => x.UserId == userId)
            .Include(x => x.Company)
            .Where(x =>
                unrestricted ||
                companyIds.Contains(x.CompanyId))
            .Select(x => x.Company)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<User>>
        GetUsersByCompanyIdAsync(
            Guid companyId,
            CancellationToken ct = default)
    {
        if (!CanAccessCompany(companyId))
            return Enumerable.Empty<User>();

        return await _db.UserCompanies
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.User)
            .Select(x => x.User)
            .ToListAsync(ct);
    }

    public async Task<bool> IsUserInCompanyAsync(
        Guid userId,
        Guid companyId,
        CancellationToken ct = default)
    {
        if (!CanAccessCompany(companyId))
            return false;

        return await _db.UserCompanies
            .AnyAsync(
                x => x.UserId == userId &&
                     x.CompanyId == companyId,
                ct);
    }

    // =============================================================
    // AUTOMATIC CODE GENERATION
    // =============================================================

    private async Task<string> GenerateAutomaticUserCodeAsync(
        Guid companyId,
        CancellationToken ct)
    {
        while (true)
        {
            var nextSequence =
                await GetNextSequenceAsync(
                    companyId,
                    ct);

            var userCode =
                FormatUserCode(nextSequence);

            // Manuel olarak daha önceden ayný Uxxxxxxx kodu verilmiþ
            // olabilir. Güvenlik için tekrar kontrol ediyoruz.
            var exists = await _db.UserCompanies
                .IgnoreQueryFilters()
                .AnyAsync(
                    x => x.CompanyId == companyId &&
                         x.UserCode == userCode,
                    ct);

            if (!exists)
                return userCode;
        }
    }

    private async Task<long> GetNextSequenceAsync(
        Guid companyId,
        CancellationToken ct)
    {
        var connection =
            _db.Database.GetDbConnection();

        var shouldCloseConnection =
            connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
            await connection.OpenAsync(ct);

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                INSERT INTO "CompanyUserCounters"
                    ("CompanyId", "LastValue")
                VALUES
                    (@companyId, 1)
                ON CONFLICT ("CompanyId")
                DO UPDATE
                SET "LastValue" =
                    "CompanyUserCounters"."LastValue" + 1
                RETURNING "LastValue";
                """;

            var companyIdParameter =
                command.CreateParameter();

            companyIdParameter.ParameterName =
                "@companyId";

            companyIdParameter.Value =
                companyId;

            command.Parameters.Add(
                companyIdParameter);

            var currentTransaction =
                _db.Database.CurrentTransaction;

            if (currentTransaction is not null)
            {
                command.Transaction =
                    currentTransaction.GetDbTransaction();
            }

            var result =
                await command.ExecuteScalarAsync(ct);

            if (result is null ||
                result == DBNull.Value)
            {
                throw new InvalidOperationException(
                    "Company user sequence could not be generated.");
            }

            var sequence =
                Convert.ToInt64(result);

            if (sequence > MaximumUserSequence)
            {
                throw new InvalidOperationException(
                    "Company user code limit has been reached. " +
                    "Maximum supported automatic code is U9999999.");
            }

            return sequence;
        }
        finally
        {
            if (shouldCloseConnection)
                await connection.CloseAsync();
        }
    }

    // =============================================================
    // MANUAL Uxxxxxxx CODE
    // =============================================================

    private async Task EnsureCounterForManualSystemCodeAsync(
        Guid companyId,
        string userCode,
        CancellationToken ct)
    {
        if (!TryParseSystemGeneratedCode(
                userCode,
                out var sequence))
        {
            return;
        }

        var connection =
            _db.Database.GetDbConnection();

        var shouldCloseConnection =
            connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
            await connection.OpenAsync(ct);

        try
        {
            await using var command =
                connection.CreateCommand();

            command.CommandText = """
                INSERT INTO "CompanyUserCounters"
                    ("CompanyId", "LastValue")
                VALUES
                    (@companyId, @sequence)
                ON CONFLICT ("CompanyId")
                DO UPDATE
                SET "LastValue" =
                    GREATEST(
                        "CompanyUserCounters"."LastValue",
                        @sequence
                    );
                """;

            var companyIdParameter =
                command.CreateParameter();

            companyIdParameter.ParameterName =
                "@companyId";

            companyIdParameter.Value =
                companyId;

            command.Parameters.Add(
                companyIdParameter);

            var sequenceParameter =
                command.CreateParameter();

            sequenceParameter.ParameterName =
                "@sequence";

            sequenceParameter.Value =
                sequence;

            command.Parameters.Add(
                sequenceParameter);

            var currentTransaction =
                _db.Database.CurrentTransaction;

            if (currentTransaction is not null)
            {
                command.Transaction =
                    currentTransaction.GetDbTransaction();
            }

            await command.ExecuteNonQueryAsync(ct);
        }
        finally
        {
            if (shouldCloseConnection)
                await connection.CloseAsync();
        }
    }

    private static bool TryParseSystemGeneratedCode(
        string userCode,
        out long sequence)
    {
        sequence = 0;

        if (string.IsNullOrWhiteSpace(userCode))
            return false;

        var normalized =
            userCode.Trim().ToUpperInvariant();

        if (normalized.Length != 8)
            return false;

        if (normalized[0] != 'U')
            return false;

        var numberPart =
            normalized.Substring(1);

        if (!numberPart.All(char.IsDigit))
            return false;

        if (!long.TryParse(
                numberPart,
                out sequence))
        {
            return false;
        }

        return sequence is >= 1 and <= MaximumUserSequence;
    }

    private static string FormatUserCode(
        long sequence)
    {
        if (sequence is < 1 or > MaximumUserSequence)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequence),
                "User sequence must be between 1 and 9999999.");
        }

        return $"U{sequence:0000000}";
    }

    private bool CanAccessCompany(
        Guid companyId)
    {
        return HasUnrestrictedScope() ||
               _currentUser
                   .GetCurrentUserCompanyIds()
                   .Contains(companyId);
    }

    private bool HasUnrestrictedScope()
    {
        return _currentUser.IsSystemUser() ||
               _currentUser
                   .GetRoles()
                   .Any(x =>
                       x.Equals(
                           "super_admin",
                           StringComparison.OrdinalIgnoreCase));
    }
}