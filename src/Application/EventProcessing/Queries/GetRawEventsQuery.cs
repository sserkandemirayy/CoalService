using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetRawEventsQuery(
    string? EventType,
    string? TagExternalId,
    string? AnchorExternalId,
    string? ProcessingStatus,
    int Page = 1,
    int PageSize = 50
) : IRequest<Result<IReadOnlyList<RawEventDto>>>;

public sealed class GetRawEventsQueryHandler : IRequestHandler<GetRawEventsQuery, Result<IReadOnlyList<RawEventDto>>>
{
    private readonly IRawEventRepository _rawEventRepository;

    public GetRawEventsQueryHandler(IRawEventRepository rawEventRepository)
    {
        _rawEventRepository = rawEventRepository;
    }

    public async Task<Result<IReadOnlyList<RawEventDto>>> Handle(GetRawEventsQuery request, CancellationToken ct)
    {
        RawEventProcessingStatus? processingStatus = null;

        if (!string.IsNullOrWhiteSpace(request.ProcessingStatus))
        {
            if (!Enum.TryParse<RawEventProcessingStatus>(request.ProcessingStatus, true, out var parsed))
                return Result<IReadOnlyList<RawEventDto>>.Failure("Invalid processing status.");

            processingStatus = parsed;
        }

        var (items, _) = await _rawEventRepository.GetPagedAsync(
            request.EventType,
            request.TagExternalId,
            request.AnchorExternalId,
            processingStatus,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new RawEventDto(
                x.Id,
                x.ExternalEventId,
                x.EventType,
                x.EventTimestamp,
                x.TagExternalId,
                x.AnchorExternalId,
                x.PayloadJson,
                x.ReceivedAt,
                x.ProcessingStatus.ToString(),
                x.ProcessedAt,
                x.ErrorMessage))
            .ToList();

        return Result<IReadOnlyList<RawEventDto>>.Success(dto);
    }
}