using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly AppDbContext _db;

    public NotificationTemplateRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<NotificationTemplate?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.NotificationTemplates.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<NotificationTemplate?> GetByCodeAsync(string code, CancellationToken ct = default)
        => await _db.NotificationTemplates
            .FirstOrDefaultAsync(x => x.Code.ToLower() == code.ToLower(), ct);

    public async Task<IReadOnlyList<NotificationTemplate>> GetAllAsync(CancellationToken ct = default)
        => await _db.NotificationTemplates
            .OrderBy(x => x.Code)
            .ToListAsync(ct);

    public async Task AddAsync(NotificationTemplate template, CancellationToken ct = default)
        => await _db.NotificationTemplates.AddAsync(template, ct);

    public Task UpdateAsync(NotificationTemplate template, CancellationToken ct = default)
    {
        _db.NotificationTemplates.Update(template);
        return Task.CompletedTask;
    }
}