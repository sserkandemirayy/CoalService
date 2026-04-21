using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Commands;

public sealed record CloseAlarmCommand(Guid AlarmId, Guid PerformedByUserId) : IRequest<Result<Guid>>;

public sealed class CloseAlarmCommandHandler : IRequestHandler<CloseAlarmCommand, Result<Guid>>
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CloseAlarmCommandHandler(IAlarmRepository alarmRepository, IUnitOfWork unitOfWork)
    {
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CloseAlarmCommand request, CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(request.AlarmId, ct);
        if (alarm is null)
            return Result<Guid>.Failure("Alarm not found.");

        alarm.Close();
        alarm.UpdateAudit(request.PerformedByUserId);

        await _alarmRepository.UpdateAsync(alarm, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(alarm.Id);
    }
}