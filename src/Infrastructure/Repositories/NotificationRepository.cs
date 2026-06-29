using Domain.Abstractions;
using Domain.Entities;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public sealed class NotificationRepository : INotificationRepository
{
    private readonly AppDbContext _db;

    public NotificationRepository(AppDbContext db)
    {
        _db = db;
    }

    public async Task<Notification?> GetByIdAsync(Guid id, CancellationToken ct = default)
        => await _db.Notifications.FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<NotificationRecipient?> GetRecipientAsync(
        Guid notificationId,
        Guid userId,
        CancellationToken ct = default)
        => await _db.NotificationRecipients
            .Include(x => x.Notification)
            .FirstOrDefaultAsync(x => x.NotificationId == notificationId && x.UserId == userId, ct);

    public async Task AddAsync(Notification notification, CancellationToken ct = default)
        => await _db.Notifications.AddAsync(notification, ct);

    public async Task AddRecipientAsync(NotificationRecipient recipient, CancellationToken ct = default)
        => await _db.NotificationRecipients.AddAsync(recipient, ct);

    public async Task<IReadOnlyList<NotificationRecipient>> GetUserNotificationsAsync(
        Guid userId,
        int page,
        int pageSize,
        CancellationToken ct = default)
        => await _db.NotificationRecipients
            .AsNoTracking()
            .Include(x => x.Notification)
            .Where(x => x.UserId == userId)
            .OrderByDescending(x => x.Notification.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(ct);

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken ct = default)
        => await _db.NotificationRecipients
            .CountAsync(x => x.UserId == userId && !x.IsRead, ct);

    public async Task<IReadOnlyList<NotificationRecipient>> GetUnreadRecipientsAsync(
        Guid userId,
        CancellationToken ct = default)
        => await _db.NotificationRecipients
            .Where(x => x.UserId == userId && !x.IsRead)
            .ToListAsync(ct);

    public Task UpdateRecipientAsync(NotificationRecipient recipient, CancellationToken ct = default)
    {
        _db.NotificationRecipients.Update(recipient);
        return Task.CompletedTask;
    }

    public IQueryable<Notification> Query() => _db.Notifications.AsQueryable();

    public IQueryable<NotificationRecipient> RecipientQuery() => _db.NotificationRecipients.AsQueryable();
}