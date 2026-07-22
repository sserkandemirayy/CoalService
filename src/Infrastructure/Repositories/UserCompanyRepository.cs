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

        if (existing is not null)
        {
            if (string.IsNullOrWhiteSpace(existing.UserCode))
            {
                var sequence = await GetNextSequenceAsync(companyId, ct);
                existing.SetUserCode(FormatUserCode(sequence));
            }

            if (existing.DeletedAt is not null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.UpdatedAt = DateTime.UtcNow;
                existing.UpdatedBy = _currentUser.GetCurrentUserId();

                _db.UserCompanies.Update(existing);
            }

            return existing.UserCode;
        }

        var nextSequence = await GetNextSequenceAsync(companyId, ct);
        var userCode = FormatUserCode(nextSequence);

        var userCompany = UserCompany.Create(
            userId,
            companyId,
            userCode);

        await _db.UserCompanies.AddAsync(userCompany, ct);

        return userCode;
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

    public async Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(
        Guid userId,
        CancellationToken ct = default)
    {
        var companyIds = _currentUser.GetCurrentUserCompanyIds();
        var unrestricted = HasUnrestrictedScope();

        return await _db.UserCompanies
            .Where(x => x.UserId == userId)
            .Include(x => x.Company)
            .Where(x =>
                unrestricted ||
                companyIds.Contains(x.CompanyId))
            .Select(x => x.Company)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(
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

    private async Task<long> GetNextSequenceAsync(
        Guid companyId,
        CancellationToken ct)
    {
        var connection = _db.Database.GetDbConnection();
        var shouldCloseConnection = connection.State != ConnectionState.Open;

        if (shouldCloseConnection)
            await connection.OpenAsync(ct);

        try
        {
            await using var command = connection.CreateCommand();

            command.CommandText = """
                INSERT INTO "CompanyUserCounters" ("CompanyId", "LastValue")
                VALUES (@companyId, 1)
                ON CONFLICT ("CompanyId")
                DO UPDATE
                SET "LastValue" = "CompanyUserCounters"."LastValue" + 1
                RETURNING "LastValue";
                """;

            var companyIdParameter = command.CreateParameter();
            companyIdParameter.ParameterName = "@companyId";
            companyIdParameter.Value = companyId;

            command.Parameters.Add(companyIdParameter);

            var currentTransaction = _db.Database.CurrentTransaction;

            if (currentTransaction is not null)
                command.Transaction = currentTransaction.GetDbTransaction();

            var result = await command.ExecuteScalarAsync(ct);

            if (result is null || result == DBNull.Value)
                throw new InvalidOperationException(
                    "Company user sequence could not be generated.");

            var sequence = Convert.ToInt64(result);

            if (sequence > MaximumUserSequence)
            {
                throw new InvalidOperationException(
                    "Company user code limit has been reached. " +
                    "Maximum supported code is U9999999.");
            }

            return sequence;
        }
        finally
        {
            if (shouldCloseConnection)
                await connection.CloseAsync();
        }
    }

    private static string FormatUserCode(long sequence)
    {
        if (sequence is < 1 or > MaximumUserSequence)
        {
            throw new ArgumentOutOfRangeException(
                nameof(sequence),
                "User sequence must be between 1 and 9999999.");
        }

        return $"U{sequence:0000000}";
    }

    private bool CanAccessCompany(Guid companyId)
    {
        return HasUnrestrictedScope() ||
               _currentUser
                   .GetCurrentUserCompanyIds()
                   .Contains(companyId);
    }

    private bool HasUnrestrictedScope()
    {
        return _currentUser.IsSystemUser() ||
               _currentUser.GetRoles().Any(
                   x => x.Equals(
                       "super_admin",
                       StringComparison.OrdinalIgnoreCase));
    }
}