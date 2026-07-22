using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmsByAnchorIdQuery(Guid AnchorId)
    : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetAlarmsByAnchorIdQueryHandler
    : IRequestHandler<
        GetAlarmsByAnchorIdQuery,
        Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmsByAnchorIdQueryHandler(
        IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(
        GetAlarmsByAnchorIdQuery request,
        CancellationToken ct)
    {
        var alarms = await _alarmRepository
            .GetByAnchorIdAsync(request.AnchorId, ct);

        return Result<IReadOnlyList<AlarmDto>>
            .Success(alarms.ToDtoList());
    }
}