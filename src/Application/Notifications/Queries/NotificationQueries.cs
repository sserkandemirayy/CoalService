using Application.Common.Models;
using Application.DTOs.Notifications;
using Domain.Abstractions;
using MediatR;

namespace Application.Notifications.Queries;

public sealed record GetNotificationsQuery(
    Guid UserId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyList<NotificationDto>>>;

public sealed class GetNotificationsQueryHandler
    : IRequestHandler<GetNotificationsQuery, Result<IReadOnlyList<NotificationDto>>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationsQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken ct)
    {
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 20 : Math.Min(request.PageSize, 100);

        var items = await _repository.GetUserNotificationsAsync(
            request.UserId,
            page,
            pageSize,
            ct);

        var dto = items
            .Select(x => new NotificationDto(
                x.Notification.Id,
                x.Notification.Title,
                x.Notification.Message,
                x.Notification.Type.ToString(),
                x.Notification.Severity.ToString(),
                x.Notification.SourceType,
                x.Notification.SourceId,
                x.Notification.ActionUrl,
                x.Notification.DataJson,
                x.IsRead,
                x.ReadAt,
                x.IsDelivered,
                x.DeliveredAt,
                x.Notification.CreatedAt))
            .ToList();

        return Result<IReadOnlyList<NotificationDto>>.Success(dto);
    }
}

public sealed record GetNotificationUnreadCountQuery(Guid UserId)
    : IRequest<Result<NotificationUnreadCountDto>>;

public sealed class GetNotificationUnreadCountQueryHandler
    : IRequestHandler<GetNotificationUnreadCountQuery, Result<NotificationUnreadCountDto>>
{
    private readonly INotificationRepository _repository;

    public GetNotificationUnreadCountQueryHandler(INotificationRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<NotificationUnreadCountDto>> Handle(GetNotificationUnreadCountQuery request, CancellationToken ct)
    {
        var count = await _repository.GetUnreadCountAsync(request.UserId, ct);
        return Result<NotificationUnreadCountDto>.Success(new NotificationUnreadCountDto(count));
    }
}