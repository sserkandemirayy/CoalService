using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.AlarmManagement.Commands;

public sealed record AddAlarmNoteCommand(
    Guid AlarmId,
    Guid UserId,
    string Note
) : IRequest<Result<Guid>>;

public sealed class AddAlarmNoteCommandHandler
    : IRequestHandler<AddAlarmNoteCommand, Result<Guid>>
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly IAlarmNoteRepository _alarmNoteRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddAlarmNoteCommandHandler(
        IAlarmRepository alarmRepository,
        IAlarmNoteRepository alarmNoteRepository,
        IUnitOfWork unitOfWork)
    {
        _alarmRepository = alarmRepository;
        _alarmNoteRepository = alarmNoteRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(
        AddAlarmNoteCommand request,
        CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(request.AlarmId, ct);

        if (alarm == null)
            return Result<Guid>.Failure("Alarm not found.");

        var note = AlarmNote.Create(
            request.AlarmId,
            request.UserId,
            request.Note);

        note.CreatedBy = request.UserId;

        await _alarmNoteRepository.AddAsync(note, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(note.Id);
    }
}