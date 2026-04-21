using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetActiveAlarmsQuery() : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetActiveAlarmsQueryHandler : IRequestHandler<GetActiveAlarmsQuery, Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetActiveAlarmsQueryHandler(IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(GetActiveAlarmsQuery request, CancellationToken ct)
    {
        var alarms = await _alarmRepository.GetActiveAlarmsAsync(ct);

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