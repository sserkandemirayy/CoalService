using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessAnchorHeartbeatReceivedCommand(AnchorHeartbeatReceivedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessAnchorHeartbeatReceivedCommandHandler : IRequestHandler<ProcessAnchorHeartbeatReceivedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly IAnchorRepository _anchorRepository;
    private readonly IAnchorHeartbeatEventRepository _anchorHeartbeatEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessAnchorHeartbeatReceivedCommandHandler(
        IRawEventRepository rawEventRepository,
        IAnchorRepository anchorRepository,
        IAnchorHeartbeatEventRepository anchorHeartbeatEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _anchorRepository = anchorRepository;
        _anchorHeartbeatEventRepository = anchorHeartbeatEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessAnchorHeartbeatReceivedCommand request, CancellationToken ct)
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

        var heartbeatEvent = AnchorHeartbeatEvent.Create(
            rawEvent.Id,
            anchor.Id,
            eventAt,
            request.Payload.IpAddress);

        await _anchorHeartbeatEventRepository.AddAsync(heartbeatEvent, ct);

        anchor.RegisterHeartbeat(eventAt, request.Payload.IpAddress);
        await _anchorRepository.UpdateAsync(anchor, ct);

        rawEvent.MarkProcessed();
        await _rawEventRepository.UpdateAsync(rawEvent, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(heartbeatEvent.Id);
    }
}