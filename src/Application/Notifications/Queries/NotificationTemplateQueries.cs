using Application.Common.Models;
using Application.DTOs.Notifications;
using Domain.Abstractions;
using MediatR;

namespace Application.Notifications.Queries;

public sealed record GetNotificationTemplatesQuery()
    : IRequest<Result<IReadOnlyList<NotificationTemplateDto>>>;

public sealed class GetNotificationTemplatesQueryHandler
    : IRequestHandler<GetNotificationTemplatesQuery, Result<IReadOnlyList<NotificationTemplateDto>>>
{
    private readonly INotificationTemplateRepository _repository;

    public GetNotificationTemplatesQueryHandler(INotificationTemplateRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<IReadOnlyList<NotificationTemplateDto>>> Handle(GetNotificationTemplatesQuery request, CancellationToken ct)
    {
        var items = await _repository.GetAllAsync(ct);

        var dto = items
            .Select(x => new NotificationTemplateDto(
                x.Id,
                x.Code,
                x.Name,
                x.TitleTemplate,
                x.MessageTemplate,
                x.ActionUrlTemplate,
                x.Type.ToString(),
                x.Severity.ToString(),
                x.IsActive))
            .ToList();

        return Result<IReadOnlyList<NotificationTemplateDto>>.Success(dto);
    }
}