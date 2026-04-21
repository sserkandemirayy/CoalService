using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetRawEventByIdQuery(Guid Id) : IRequest<Result<RawEventDto>>;

public sealed class GetRawEventByIdQueryHandler : IRequestHandler<GetRawEventByIdQuery, Result<RawEventDto>>
{
    private readonly IRawEventRepository _rawEventRepository;

    public GetRawEventByIdQueryHandler(IRawEventRepository rawEventRepository)
    {
        _rawEventRepository = rawEventRepository;
    }

    public async Task<Result<RawEventDto>> Handle(GetRawEventByIdQuery request, CancellationToken ct)
    {
        var rawEvent = await _rawEventRepository.GetByIdAsync(request.Id, ct);
        if (rawEvent is null)
            return Result<RawEventDto>.Failure("Raw event not found.");

        var dto = new RawEventDto(
            rawEvent.Id,
            rawEvent.ExternalEventId,
            rawEvent.EventType,
            rawEvent.EventTimestamp,
            rawEvent.TagExternalId,
            rawEvent.AnchorExternalId,
            rawEvent.PayloadJson,
            rawEvent.ReceivedAt,
            rawEvent.ProcessingStatus.ToString(),
            rawEvent.ProcessedAt,
            rawEvent.ErrorMessage);

        return Result<RawEventDto>.Success(dto);
    }
}