using Application.Common.Models;
using Application.DTOs.EventProcessing;
using Domain.Abstractions;
using MediatR;

namespace Application.EventProcessing.Queries;

public sealed record GetEmergencyEventsByTagIdQuery(
    Guid TagId,
    int Page = 1,
    int PageSize = 100
) : IRequest<Result<IReadOnlyList<EmergencyEventDto>>>;

public sealed class GetEmergencyEventsByTagIdQueryHandler : IRequestHandler<GetEmergencyEventsByTagIdQuery, Result<IReadOnlyList<EmergencyEventDto>>>
{
    private readonly IEmergencyEventRepository _emergencyEventRepository;

    public GetEmergencyEventsByTagIdQueryHandler(IEmergencyEventRepository emergencyEventRepository)
    {
        _emergencyEventRepository = emergencyEventRepository;
    }

    public async Task<Result<IReadOnlyList<EmergencyEventDto>>> Handle(GetEmergencyEventsByTagIdQuery request, CancellationToken ct)
    {
        var (items, _) = await _emergencyEventRepository.GetPagedByTagIdAsync(
            request.TagId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new EmergencyEventDto(
                x.Id,
                x.RawEventId,
                x.TagId,
                x.EventTimestamp))
            .ToList();

        return Result<IReadOnlyList<EmergencyEventDto>>.Success(dto);
    }
}