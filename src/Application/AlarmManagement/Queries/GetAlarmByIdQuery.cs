using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmByIdQuery(Guid AlarmId)
    : IRequest<Result<AlarmDto>>;

public sealed class GetAlarmByIdQueryHandler
    : IRequestHandler<GetAlarmByIdQuery, Result<AlarmDto>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmByIdQueryHandler(
        IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<AlarmDto>> Handle(
        GetAlarmByIdQuery request,
        CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(
            request.AlarmId,
            ct);

        if (alarm is null)
            return Result<AlarmDto>.Failure("Alarm not found.");

        return Result<AlarmDto>.Success(alarm.ToDto());
    }
}