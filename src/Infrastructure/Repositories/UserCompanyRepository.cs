using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserCompanyRepository : IUserCompanyRepository
{
    private readonly AppDbContext _db;
    public UserCompanyRepository(AppDbContext db) => _db = db;

    public async Task AddOrReactivateAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        // Soft-delete veya mevcut kayıt var mı kontrol et
        var existing = await _db.UserCompanies
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);

        if (existing != null)
        {
            // Eğer soft deleted ise reaktif et
            if (existing.DeletedAt != null)
            {
                existing.DeletedAt = null;
                existing.DeletedBy = null;
                existing.UpdatedAt = DateTime.UtcNow;
                _db.UserCompanies.Update(existing);
            }
            // aktifse zaten bir şey yapmaya gerek yok
            return;
        }

        // Yeni kayıt oluştur
        await _db.UserCompanies.AddAsync(UserCompany.Create(userId, companyId), ct);
    }

    //public async Task RemoveAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    //{
    //    var link = await _db.UserCompanies
    //        .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);

    //    if (link is null)
    //        return;

    //    link.DeletedAt = DateTime.UtcNow;
    //    link.DeletedBy = userId; 
    //    link.UpdatedAt = DateTime.UtcNow;
    //    link.UpdatedBy = userId; 

    //    _db.UserCompanies.Update(link);
    //}

    public async Task RemoveAsync(Guid userId, Guid companyId, Guid performedBy, CancellationToken ct = default)
    {
        // 1) UserCompany kaydını bul
        var link = await _db.UserCompanies
            .FirstOrDefaultAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);

        if (link is null)
            return;

        // 2) Soft-delete
        link.DeletedAt = DateTime.UtcNow;
        link.DeletedBy = performedBy;
        link.UpdatedAt = DateTime.UtcNow;
        link.UpdatedBy = performedBy;

        _db.UserCompanies.Update(link);

        // 3) Bu company’e ait tüm branchleri bul
        var branchIds = await _db.Branches
            .Where(b => b.CompanyId == companyId)
            .Select(b => b.Id)
            .ToListAsync(ct);

        // 4) User’ın bu branchlerdeki tüm yetkilerini sil
        var userBranches = await _db.UserBranches
            .Where(ub => ub.UserId == userId && branchIds.Contains(ub.BranchId) && ub.DeletedAt == null)
            .ToListAsync(ct);

        foreach (var ub in userBranches)
        {
            ub.DeletedAt = DateTime.UtcNow;
            ub.DeletedBy = performedBy;
            ub.UpdatedAt = DateTime.UtcNow;
            ub.UpdatedBy = performedBy;
        }

        _db.UserBranches.UpdateRange(userBranches);
    }

    public async Task<IEnumerable<Company>> GetCompaniesByUserIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.UserCompanies
            .Where(x => x.UserId == userId)
            .Include(x => x.Company)
            .Select(x => x.Company)
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<User>> GetUsersByCompanyIdAsync(Guid companyId, CancellationToken ct = default)
    {
        return await _db.UserCompanies
            .Where(x => x.CompanyId == companyId)
            .Include(x => x.User)
            .Select(x => x.User)
            .ToListAsync(ct);
    }

    public async Task<bool> IsUserInCompanyAsync(Guid userId, Guid companyId, CancellationToken ct = default)
    {
        return await _db.UserCompanies
            .AnyAsync(x => x.UserId == userId && x.CompanyId == companyId, ct);
    }
}