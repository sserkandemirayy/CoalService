using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessImuEventDetectedCommand(ImuEventDetectedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessImuEventDetectedCommandHandler : IRequestHandler<ProcessImuEventDetectedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IImuEventRepository _imuEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessImuEventDetectedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        IImuEventRepository imuEventRepository,
        IAlarmRepository alarmRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _imuEventRepository = imuEventRepository;
        _alarmRepository = alarmRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessImuEventDetectedCommand request, CancellationToken ct)
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
            await _rawEventRepository.UpdateAsync(rawEvent, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var imuType = EventProcessingHelper.ParseImuEventType(request.Payload.EventType);

        var imuEvent = ImuEvent.Create(rawEvent.Id, tag.Id, eventAt, imuType);
        await _imuEventRepository.AddAsync(imuEvent, ct);

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(tag.Id, ct);

        var alarmType = imuType switch
        {
            ImuEventType.FallDetected => AlarmType.FallDetected,
            ImuEventType.InactivityDetected => AlarmType.InactivityDetected,
            ImuEventType.ShockDetected => AlarmType.ShockDetected,
            ImuEventType.AbnormalPositionDetected => AlarmType.AbnormalPositionDetected,
            _ => AlarmType.AbnormalPositionDetected
        };

        var alarmSeverity = imuType switch
        {
            ImuEventType.FallDetected => AlarmSeverity.Critical,
            ImuEventType.ShockDetected => AlarmSeverity.Critical,
            ImuEventType.InactivityDetected => AlarmSeverity.Warning,
            ImuEventType.AbnormalPositionDetected => AlarmSeverity.Warning,
            _ => AlarmSeverity.Warning
        };

        var alarm = Alarm.Create(
            alarmType,
            alarmSeverity,
            $"IMU event: {imuType}",
            eventAt,
            rawEvent.Id,
            tag.Id,
            null,
            null,
            activeAssignment?.UserId,
            "IMU safety event detected.",
            EventProcessingHelper.Serialize(request.Payload));

        await _alarmRepository.AddAsync(alarm, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        rawEvent.MarkProcessed();
        await _rawEventRepository.UpdateAsync(rawEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(imuEvent.Id);
    }
}