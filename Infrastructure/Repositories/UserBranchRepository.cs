using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

public class UserBranchRepository : IUserBranchRepository
{
    private readonly AppDbContext _db;
    public UserBranchRepository(AppDbContext db) => _db = db;

    // --- USER - BRANCH EKLE / REACTIVATE ---
    public async Task AddOrReactivateAsync(Guid userId, Guid branchId, Guid performedBy, CancellationToken ct = default)
    {
        var entity = await _db.UserBranches
            .IgnoreQueryFilters()   // ❗ Soft deleted kayıtları da gör
            .FirstOrDefaultAsync(x => x.UserId == userId && x.BranchId == branchId, ct);

        if (entity == null)
        {
            // tamamen yeni ilişki ekle
            await _db.UserBranches.AddAsync(UserBranch.Create(userId, branchId, performedBy), ct);
        }
        else
        {
            // daha önce silinmişse aktif et
            if (entity.DeletedAt != null)
            {
                entity.DeletedAt = null;
                entity.DeletedBy = null;
                entity.UpdatedAt = DateTime.UtcNow;
                entity.UpdatedBy = performedBy;

                _db.UserBranches.Update(entity);
            }
            // ❗ Aktifse hiçbir şey yapma
        }
    }

    // --- USER - BRANCH SİL (SOFT DELETE) ---
    public async Task RemoveAsync(Guid userId, Guid branchId, Guid performedBy, CancellationToken ct = default)
    {
        var entity = await _db.UserBranches
            .IgnoreQueryFilters()  // ❗ Soft deleted olanı da gör
            .FirstOrDefaultAsync(x => x.UserId == userId && x.BranchId == branchId, ct);

        if (entity != null && entity.DeletedAt == null)
        {
            entity.DeletedAt = DateTime.UtcNow;
            entity.DeletedBy = performedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            entity.UpdatedBy = performedBy;

            _db.UserBranches.Update(entity);
        }
    }

    // --- User’ın BRANCH ID listesi ---
    public async Task<IEnumerable<Guid>> GetUserBranchIdsAsync(Guid userId, CancellationToken ct = default)
        => await _db.UserBranches
            .Where(x => x.UserId == userId)
            .Select(x => x.BranchId)
            .ToListAsync(ct);

    // --- User’ın tüm branch entity listesi ---
    public async Task<IEnumerable<Branch>> GetBranchesByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.UserBranches
            .Where(x => x.UserId == userId)
            .Select(x => x.Branch)
            .AsNoTracking()
            .ToListAsync(ct);

    // --- Branch’a bağlı kullanıcılar ---
    public async Task<IEnumerable<User>> GetUsersByBranchIdAsync(Guid branchId, CancellationToken ct = default)
        => await _db.UserBranches
            .Where(x => x.BranchId == branchId)
            .Select(x => x.User)
            .AsNoTracking()
            .ToListAsync(ct);
}