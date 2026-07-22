using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetActiveAlarmsQuery()
    : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetActiveAlarmsQueryHandler
    : IRequestHandler<
        GetActiveAlarmsQuery,
        Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetActiveAlarmsQueryHandler(
        IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(
        GetActiveAlarmsQuery request,
        CancellationToken ct)
    {
        var alarms = await _alarmRepository
            .GetActiveAlarmsAsync(ct);

        return Result<IReadOnlyList<AlarmDto>>
            .Success(alarms.ToDtoList());
    }
}