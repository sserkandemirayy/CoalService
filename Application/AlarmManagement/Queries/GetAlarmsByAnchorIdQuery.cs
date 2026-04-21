using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmsByAnchorIdQuery(Guid AnchorId) : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetAlarmsByAnchorIdQueryHandler : IRequestHandler<GetAlarmsByAnchorIdQuery, Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmsByAnchorIdQueryHandler(IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(GetAlarmsByAnchorIdQuery request, CancellationToken ct)
    {
        var alarms = await _alarmRepository.GetByAnchorIdAsync(request.AnchorId, ct);

        var items = alarms
            .Select(alarm => new AlarmDto(
                alarm.Id,
                alarm.RawEventId,
                alarm.AlarmType.ToString(),
                alarm.Severity.ToString(),
                alarm.Status.ToString(),
                alarm.TagId,
                alarm.PeerTagId,
                alarm.AnchorId,
                alarm.UserId,
                alarm.StartedAt,
                alarm.EndedAt,
                alarm.AcknowledgedAt,
                alarm.AcknowledgedBy,
                alarm.Title,
                alarm.Description,
                alarm.DataJson))
            .ToList();

        return Result<IReadOnlyList<AlarmDto>>.Success(items);
    }
}