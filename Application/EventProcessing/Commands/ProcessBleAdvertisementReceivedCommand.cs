using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessBleAdvertisementReceivedCommand(BleAdvertisementReceivedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessBleAdvertisementReceivedCommandHandler : IRequestHandler<ProcessBleAdvertisementReceivedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IBleAdvertisementEventRepository _bleAdvertisementEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessBleAdvertisementReceivedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        ITagRepository tagRepository,
        IBleAdvertisementEventRepository bleAdvertisementEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _tagRepository = tagRepository;
        _bleAdvertisementEventRepository = bleAdvertisementEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessBleAdvertisementReceivedCommand request, CancellationToken ct)
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
            request.Payload.AnchorId);

        await _rawEventRepository.AddAsync(rawEvent, ct);

        var anchor = await _anchorRepository.GetByExternalIdAsync(request.Payload.AnchorId, ct);
        if (anchor is null)
        {
            rawEvent.MarkFailed("Anchor not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Anchor not found.");
        }

        var tag = await _tagRepository.GetByExternalIdAsync(request.Payload.TagId, ct);
        if (tag is null)
        {
            rawEvent.MarkFailed("Tag not found.");
            await _unitOfWork.SaveChangesAsync(ct);
            return Result<Guid>.Failure("Tag not found.");
        }

        var telemetryEvent = BleAdvertisementEvent.Create(
            rawEvent.Id,
            anchor.Id,
            tag.Id,
            eventAt,
            request.Payload.Rssi);

        await _bleAdvertisementEventRepository.AddAsync(telemetryEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        anchor.RegisterHeartbeat(eventAt);
        await _anchorRepository.UpdateAsync(anchor, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(telemetryEvent.Id);
    }
}