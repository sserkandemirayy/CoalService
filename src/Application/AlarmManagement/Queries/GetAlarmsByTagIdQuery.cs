using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmsByTagIdQuery(Guid TagId)
    : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetAlarmsByTagIdQueryHandler
    : IRequestHandler<
        GetAlarmsByTagIdQuery,
        Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmsByTagIdQueryHandler(
        IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(
        GetAlarmsByTagIdQuery request,
        CancellationToken ct)
    {
        var alarms = await _alarmRepository
            .GetByTagIdAsync(request.TagId, ct);

        return Result<IReadOnlyList<AlarmDto>>
            .Success(alarms.ToDtoList());
    }
}