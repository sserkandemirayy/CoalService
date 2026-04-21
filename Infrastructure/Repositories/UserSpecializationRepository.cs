using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserSpecializationRepository : IUserSpecializationRepository
{
    private readonly AppDbContext _db;
    public UserSpecializationRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<UserSpecialization>> GetAllAsync(CancellationToken ct = default)
        => await _db.UserSpecializations
            .Include(x => x.UserType)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task<UserSpecialization?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.UserSpecializations
             .Include(x => x.UserType)
             .FirstOrDefaultAsync(x => x.Id == id, ct); 

    public async Task<IEnumerable<UserSpecialization>> GetByUserTypeIdAsync(Guid userTypeId, CancellationToken ct = default)
        => await _db.UserSpecializations
            .Include(x => x.UserType)
            .Where(x => x.UserTypeId == userTypeId)
            .AsNoTracking()
            .ToListAsync(ct);

    public async Task AddAsync(UserSpecialization entity, CancellationToken ct = default)
        => await _db.UserSpecializations.AddAsync(entity, ct);

    public async Task UpdateAsync(UserSpecialization entity, CancellationToken ct = default)
    {
        var existing = await _db.UserSpecializations
            .FirstOrDefaultAsync(x => x.Id == entity.Id, ct);

        if (existing == null)
            return;

        // code aynı kalabilir ama başka bir specialization’da olamaz
        bool codeExists = await _db.UserSpecializations
            .AnyAsync(x => x.Code == entity.Code && x.Id != entity.Id, ct);

        if (codeExists)
            throw new InvalidOperationException("Specialization code already exists.");

        // UPDATE (UserSpecialization.Update(name, desc))
        existing.Update(entity.Name, entity.Description);

        _db.UserSpecializations.Update(existing);
    }

    public async Task RemoveAsync(Guid id, Guid performedBy, CancellationToken ct = default)
    {
        var spec = await _db.UserSpecializations
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

        if (spec == null)
            return;

        if (spec.IsSystem)
            throw new InvalidOperationException("System specialization cannot be deleted.");

        // aktif kullanıcı bağlıysa silinemez
        bool hasActiveUsers = await _db.Users
            .AnyAsync(u => u.SpecializationId == id && u.DeletedAt == null, ct);

        if (hasActiveUsers)
            throw new InvalidOperationException("Specialization cannot be deleted because it is assigned to active users.");

        // soft deleted kullanıcılardan referansı kaldır
        var softDeletedUsers = await _db.Users
            .Where(u => u.SpecializationId == id && u.DeletedAt != null)
            .ToListAsync(ct);

        foreach (var user in softDeletedUsers)
        {
            user.SetSpecialization(null);
            _db.Users.Update(user);
        }

        // specialization'ı soft delete et
        spec.DeletedAt = DateTime.UtcNow;
        spec.DeletedBy = performedBy;

        _db.UserSpecializations.Update(spec);
    }

    public async Task<bool> HasUsersAsync(Guid specializationId, CancellationToken ct = default)
        => await _db.Users.AnyAsync(u => u.SpecializationId == specializationId && u.DeletedAt == null, ct);
}