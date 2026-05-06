using Domain.Abstractions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services;

public class DataRetentionService : IDataRetentionService
{
    private readonly AppDbContext _db;

    public DataRetentionService(AppDbContext db)
    {
        _db = db;
    }

    // Basit örnek: 2 yıldan eski silinmiş kayıtları kalıcı sil
    public async Task RunAsync(CancellationToken ct = default)
    {
        var cutoff = DateTime.UtcNow.AddYears(-2);

        var oldDeletedUsers = await _db.Users
            .Where(u => u.DeletedAt != null && u.DeletedAt < cutoff)
            .ToListAsync(ct);

        _db.Users.RemoveRange(oldDeletedUsers);
        await _db.SaveChangesAsync(ct);
    }
}
