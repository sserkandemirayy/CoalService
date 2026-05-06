using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmByIdQuery(Guid AlarmId) : IRequest<Result<AlarmDto>>;

public sealed class GetAlarmByIdQueryHandler : IRequestHandler<GetAlarmByIdQuery, Result<AlarmDto>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmByIdQueryHandler(IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<AlarmDto>> Handle(GetAlarmByIdQuery request, CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(request.AlarmId, ct);
        if (alarm is null)
            return Result<AlarmDto>.Failure("Alarm not found.");

        var dto = new AlarmDto(
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
            alarm.DataJson);

        return Result<AlarmDto>.Success(dto);
    }
}