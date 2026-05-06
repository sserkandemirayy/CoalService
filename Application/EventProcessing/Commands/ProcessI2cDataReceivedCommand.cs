using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.EventProcessing.Commands;

public sealed record ProcessI2cDataReceivedCommand(I2cDataReceivedPayloadDto Payload) : IRequest<Result<Guid>>;

public sealed class ProcessI2cDataReceivedCommandHandler : IRequestHandler<ProcessI2cDataReceivedCommand, Result<Guid>>
{
    private readonly IRawEventRepository _rawEventRepository;
    private readonly ITagRepository _tagRepository;
    private readonly II2cDataEventRepository _i2cDataEventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ProcessI2cDataReceivedCommandHandler(
        IRawEventRepository rawEventRepository,
        ITagRepository tagRepository,
        II2cDataEventRepository i2cDataEventRepository,
        IUnitOfWork unitOfWork)
    {
        _rawEventRepository = rawEventRepository;
        _tagRepository = tagRepository;
        _i2cDataEventRepository = i2cDataEventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ProcessI2cDataReceivedCommand request, CancellationToken ct)
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

        var direction = EventProcessingHelper.ParseI2cDataDirection(request.Payload.Direction);
        var dataJson = EventProcessingHelper.Serialize(request.Payload.Data);

        var telemetryEvent = I2cDataEvent.Create(
            rawEvent.Id,
            tag.Id,
            eventAt,
            request.Payload.Address,
            request.Payload.Register,
            direction,
            request.Payload.Ack,
            dataJson);

        await _i2cDataEventRepository.AddAsync(telemetryEvent, ct);

        tag.MarkSeen(eventAt);
        await _tagRepository.UpdateAsync(tag, ct);

        rawEvent.MarkProcessed();
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(telemetryEvent.Id);
    }
}