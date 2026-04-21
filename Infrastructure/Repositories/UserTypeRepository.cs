using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserTypeRepository : IUserTypeRepository
{
    private readonly AppDbContext _db;
    public UserTypeRepository(AppDbContext db) => _db = db;

    public async Task<IEnumerable<UserType>> GetAllAsync(CancellationToken ct = default)
        => await _db.UserTypes.AsNoTracking().ToListAsync(ct);

    public async Task<UserType?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.UserTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<UserType?> FindByCodeAsync(string name, CancellationToken cancellationToken = default)
    {
        //return await _db.UserTypes.AsNoTracking().FirstOrDefaultAsync(r => r.Code.ToLowerInvariant() == name.ToLowerInvariant(), cancellationToken);

        var normalizedCode = (name ?? "").Trim().ToUpperInvariant();

        return await _db.UserTypes
          .AsNoTracking()
          .FirstOrDefaultAsync(u => u.Code == normalizedCode, cancellationToken);
    }
}