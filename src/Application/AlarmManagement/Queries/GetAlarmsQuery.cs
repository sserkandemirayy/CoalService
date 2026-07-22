using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmsQuery(
    string? Status,
    DateTime? StartDate,
    DateTime? EndDate)
    : IRequest<Result<IReadOnlyList<AlarmDto>>>;

public sealed class GetAlarmsQueryHandler
    : IRequestHandler<
        GetAlarmsQuery,
        Result<IReadOnlyList<AlarmDto>>>
{
    private readonly IAlarmRepository _alarmRepository;

    public GetAlarmsQueryHandler(
        IAlarmRepository alarmRepository)
    {
        _alarmRepository = alarmRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmDto>>> Handle(
        GetAlarmsQuery request,
        CancellationToken ct)
    {
        AlarmStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            var statusParsed = Enum.TryParse<AlarmStatus>(
                request.Status.Trim(),
                ignoreCase: true,
                out var parsedStatus);

            if (!statusParsed)
            {
                var validStatuses = string.Join(
                    ", ",
                    Enum.GetNames<AlarmStatus>());

                return Result<IReadOnlyList<AlarmDto>>.Failure(
                    $"Invalid alarm status. Valid values: {validStatuses}.");
            }

            status = parsedStatus;
        }

        if (request.StartDate.HasValue &&
            request.EndDate.HasValue &&
            request.StartDate.Value > request.EndDate.Value)
        {
            return Result<IReadOnlyList<AlarmDto>>.Failure(
                "StartDate cannot be later than EndDate.");
        }

        var alarms = await _alarmRepository.GetAllAsync(
            status,
            request.StartDate,
            request.EndDate,
            ct);

        return Result<IReadOnlyList<AlarmDto>>
            .Success(alarms.ToDtoList());
    }
}