using Application.Common.Models;
using Application.DTOs.Tracking;
using Application.Tracking.Mappings;
using Domain.Abstractions;
using MediatR;

namespace Application.Tracking.Queries;

public sealed record GetCurrentLocationsQuery(
    Guid? UserId = null,
    Guid? TagId = null)
    : IRequest<Result<IReadOnlyList<CurrentLocationDto>>>;

public sealed class GetCurrentLocationsQueryHandler
    : IRequestHandler<
        GetCurrentLocationsQuery,
        Result<IReadOnlyList<CurrentLocationDto>>>
{
    private readonly ICurrentLocationRepository
        _currentLocationRepository;

    public GetCurrentLocationsQueryHandler(
        ICurrentLocationRepository currentLocationRepository)
    {
        _currentLocationRepository =
            currentLocationRepository;
    }

    public async Task<
        Result<IReadOnlyList<CurrentLocationDto>>> Handle(
        GetCurrentLocationsQuery request,
        CancellationToken ct)
    {
        var items =
            await _currentLocationRepository.GetFilteredAsync(
                request.UserId,
                request.TagId,
                ct);

        return Result<IReadOnlyList<CurrentLocationDto>>
            .Success(items.ToDtoList());
    }
}