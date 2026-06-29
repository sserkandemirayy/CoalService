using Domain.Enums;

namespace Application.Common.Notifications;

public interface INotificationTargetResolver
{
    Task<IReadOnlyList<Guid>> ResolveAsync(
        NotificationTargetType targetType,
        IReadOnlyList<Guid>? targetIds,
        CancellationToken ct = default);

    Task<IReadOnlyList<Guid>> ResolveByPermissionAsync(
        string permissionName,
        CancellationToken ct = default);
}