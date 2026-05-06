using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessEmergencyButtonPressedCommand(EmergencyButtonPressedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessEmergencyButtonPressedCommandHandler : IRequestHandler<ProcessEmergencyButtonPressedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IEmergencyEventRepository _emergencyEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessEmergencyButtonPressedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IEmergencyEventRepository emergencyEventRepository,
        IAlarmRepository alarmRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _emergencyEventRepository = emergencyEventRepository;
        _alarmRepository = alarmRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessEmergencyButtonPressedCommand request, CancellationToken ct)
    {
        if (await _rawEventRepository.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var rawEvent = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            request.Payload.TagId,
            null);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var tag = await _tagRepository.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
        {
            rawEvent.MarkFailed("Tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var emergencyEvent = EmergencyEvent.Create(rawEvent.Id, tag.Id, eventAt);
        await _emergencyEventRepository.AddAsync(emergencyEvent, ct);

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);

        var alarm = Alarm.Create(
            AlarmType.EmergencyButtonPressed,
            AlarmSeverity.Critical,
            "Emergency button pressed",
            eventAt,
            rawEvent.Id,
            tag.Id,
            null,
            null,
            activeAssignment?.UserId,
            "Emergency button press received from tag.",
            EventProcessingHelper.Serialize(request.Payload));

        await _alarmRepository.AddAsync(alarm, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(emergencyEvent.Id);
    }
}