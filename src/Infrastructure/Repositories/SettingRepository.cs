using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class SettingRepository : ISettingRepository
{
    private readonly AppDbContext _db;
    public SettingRepository(AppDbContext db) => _db = db;

    public async Task<Setting?> GetAsync(string key, Guid? userId = null, CancellationToken ct = default)
    {
        return await _db.Settings.FirstOrDefaultAsync(s =>
            s.Key == key && (s.Scope == SettingScope.System || s.UserId == userId), ct);
    }

    public async Task<IEnumerable<Setting>> GetAllAsync(SettingScope scope, Guid? userId = null, CancellationToken ct = default)
    {
        return await _db.Settings
            .Where(s => s.Scope == scope && (scope == SettingScope.System || s.UserId == userId))
            .ToListAsync(ct);
    }

    public async Task AddAsync(Setting setting, CancellationToken ct = default)
        => await _db.Settings.AddAsync(setting, ct);

    public async Task UpdateAsync(Setting setting, CancellationToken ct = default)
    {
        _db.Settings.Update(setting);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Setting setting, CancellationToken ct = default)
    {
        _db.Settings.Remove(setting);
        await Task.CompletedTask;
    }
}
