using Application.AlarmManagement.Mappings;
using Application.Common.Models;
using Application.DTOs.AlarmManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Queries;

public sealed record GetAlarmNotesQuery(Guid AlarmId)
    : IRequest<Result<IReadOnlyList<AlarmNoteDto>>>;

public sealed class GetAlarmNotesQueryHandler
    : IRequestHandler<
        GetAlarmNotesQuery,
        Result<IReadOnlyList<AlarmNoteDto>>>
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly IAlarmNoteRepository _alarmNoteRepository;

    public GetAlarmNotesQueryHandler(
        IAlarmRepository alarmRepository,
        IAlarmNoteRepository alarmNoteRepository)
    {
        _alarmRepository = alarmRepository;
        _alarmNoteRepository = alarmNoteRepository;
    }

    public async Task<Result<IReadOnlyList<AlarmNoteDto>>> Handle(
        GetAlarmNotesQuery request,
        CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(
            request.AlarmId,
            ct);

        if (alarm is null)
        {
            return Result<IReadOnlyList<AlarmNoteDto>>
                .Failure("Alarm not found.");
        }

        var alarmNotes = await _alarmNoteRepository
            .GetByAlarmIdAsync(request.AlarmId, ct);

        return Result<IReadOnlyList<AlarmNoteDto>>
            .Success(alarmNotes.ToDtoList());
    }
}