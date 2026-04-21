using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessAnchorHealthReportedCommand(AnchorHealthReportedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessAnchorHealthReportedCommandHandler : IRequestHandler<ProcessAnchorHealthReportedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IAnchorHealthEventRepository _anchorHealthEventRepository;
    private readonly IAlarmRepository _alarmRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessAnchorHealthReportedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        IAnchorHealthEventRepository anchorHealthEventRepository,
        IAlarmRepository alarmRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _anchorHealthEventRepository = anchorHealthEventRepository;
        _alarmRepository = alarmRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessAnchorHealthReportedCommand request, CancellationToken ct)
    {
        if (await _rawEventRepository.ExistsByExternalEventIdAsync(request.Payload.Id, ct))
            return Result<Guid>.Failure("Duplicate raw event.");

        var eventAt = EventProcessingHelper.FromUnixMilliseconds(request.Payload.Timestamp);

        var rawEvent = RawEvent.Create(
            request.Payload.Id,
            request.Payload.Type,
            eventAt,
            EventProcessingHelper.Serialize(request.Payload),
            null,
            request.Payload.AnchorId);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var anchor = await _anchorRepository.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
        {
            rawEvent.MarkFailed("Anchor not found.");
            await _rawEventRepository.UpdateAsync(rawEvent, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Anchor not found.");
        }

        var healthEvent = AnchorHealthEvent.Create(
            rawEvent.Id,
            anchor.Id,
            eventAt,
            request.Payload.Uptime,
            request.Payload.Temperature,
            request.Payload.CpuUsage,
            request.Payload.MemoryUsage,
            request.Payload.TagCount,
            request.Payload.PacketLossRate);

        await _anchorHealthEventRepository.AddAsync(healthEvent, ct);

        anchor.ChangeStatus(AnchorStatus.Online, eventAt);
        await _anchorRepository.UpdateAsync(anchor, ct);

        if (request.Payload.PacketLossRate >= 40 || request.Payload.CpuUsage >= 95 || request.Payload.MemoryUsage >= 95)
        {
            var severity = request.Payload.PacketLossRate >= 70 || request.Payload.CpuUsage >= 99 || request.Payload.MemoryUsage >= 99
                ? AlarmSeverity.Critical
                : AlarmSeverity.Warning;

            var existing = await _alarmRepository.HasActiveAlarmAsync(
                AlarmType.AnchorError,
                anchorId: anchor.Id,
                ct: ct);

            if (!existing)
            {
                var alarm = Alarm.Create(
                    AlarmType.AnchorError,
                    severity,
                    "Anchor health degraded",
                    eventAt,
                    rawEvent.Id,
                    null,
                    null,
                    anchor.Id,
                    null,
                    "Anchor health metrics exceeded threshold.",
                    EventProcessingHelper.Serialize(request.Payload));

                await _alarmRepository.AddAsync(alarm, ct);
            }
        }

        rawEvent.MarkProcessed();
        await _rawEventRepository.UpdateAsync(rawEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(healthEvent.Id);
    }
}