using Application.Common.Models;
using Application.DTOs.Tracking;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetCurrentLocationByTagIdQuery(Guid TagId) : IRequest<Result<CurrentLocationDto>>;

public sealed class GetCurrentLocationByTagIdQueryHandler : IRequestHandler<GetCurrentLocationByTagIdQuery, Result<CurrentLocationDto>>
{
    private readonly ICurrentLocationRepository _currentLocationRepository;

    public GetCurrentLocationByTagIdQueryHandler(ICurrentLocationRepository currentLocationRepository)
    {
        _currentLocationRepository = currentLocationRepository;
    }

    public async Task<Result<CurrentLocationDto>> Handle(GetCurrentLocationByTagIdQuery request, CancellationToken ct)
    {
        var currentLocation = await _currentLocationRepository.GetByTagIdAsync(request.TagId, ct);
        if (currentLocation is null)
            return Result<CurrentLocationDto>.Failure("Current location not found.");

        var dto = new CurrentLocationDto(
            currentLocation.Id,
            currentLocation.TagId,
            currentLocation.UserId,
            currentLocation.X,
            currentLocation.Y,
            currentLocation.Z,
            currentLocation.Accuracy,
            currentLocation.Confidence,
            currentLocation.LastEventAt,
            currentLocation.LastRawEventId,
            currentLocation.LastKnownAnchorCount);

        return Result<CurrentLocationDto>.Success(dto);
    }
}