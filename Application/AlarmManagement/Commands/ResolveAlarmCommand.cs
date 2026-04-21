using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.AlarmManagement.Commands;

public sealed record ResolveAlarmCommand(Guid AlarmId, Guid PerformedByUserId) : IRequest<Result<Guid>>;

public sealed class ResolveAlarmCommandHandler : IRequestHandler<ResolveAlarmCommand, Result<Guid>>
{
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ResolveAlarmCommandHandler(IAlarmRepository alarmRepository, IUnitOfWork unitOfWork)
    {
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ResolveAlarmCommand request, CancellationToken ct)
    {
        var alarm = await _alarmRepository.GetByIdAsync(request.AlarmId, ct);
        if (alarm is null)
            return Result<Guid>.Failure("Alarm not found.");

        alarm.Resolve();
        alarm.UpdateAudit(request.PerformedByUserId);

        await _alarmRepository.UpdateAsync(alarm, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(alarm.Id);
    }
}