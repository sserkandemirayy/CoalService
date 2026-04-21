using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using System.Text.Json;

namespace Application.Tracking.Commands;

public sealed record RebuildCurrentLocationCommand(Guid TagId) : IRequest<Result<Guid>>;

public sealed class RebuildCurrentLocationCommandHandler : IRequestHandler<RebuildCurrentLocationCommand, Result<Guid>>
{
    private readonly ILocationEventRepository _locationEventRepository;
    private readonly ICurrentLocationRepository _currentLocationRepository;
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RebuildCurrentLocationCommandHandler(
        ILocationEventRepository locationEventRepository,
        ICurrentLocationRepository currentLocationRepository,
        ITagAssignmentRepository tagAssignmentRepository,
        IUnitOfWork unitOfWork)
    {
        _locationEventRepository = locationEventRepository;
        _currentLocationRepository = currentLocationRepository;
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(RebuildCurrentLocationCommand request, CancellationToken ct)
    {
        var lastLocation = await _locationEventRepository.GetLatestByTagIdAsync(request.TagId, ct);

        if (lastLocation is null)
            return Result<Guid>.Failure("No location event found for tag.");

        var activeAssignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(request.TagId, ct);
        var currentLocation = await _currentLocationRepository.GetByTagIdAsync(request.TagId, ct);

        int anchorCount = 0;
        try
        {
            using var document = JsonDocument.Parse(lastLocation.UsedAnchorsJson);
            if (document.RootElement.ValueKind == JsonValueKind.Array)
                anchorCount = document.RootElement.GetArrayLength();
        }
        catch
        {
            anchorCount = 0;
        }

        if (currentLocation is null)
        {
            currentLocation = CurrentLocation.Create(
                lastLocation.TagId,
                activeAssignment?.UserId,
                lastLocation.X,
                lastLocation.Y,
                lastLocation.Z,
                lastLocation.Accuracy,
                lastLocation.Confidence,
                lastLocation.EventTimestamp,
                lastLocation.RawEventId,
                anchorCount);

            await _currentLocationRepository.AddAsync(currentLocation, ct);
        }
        else
        {
            currentLocation.UpdateFromLocation(
                activeAssignment?.UserId,
                lastLocation.X,
                lastLocation.Y,
                lastLocation.Z,
                lastLocation.Accuracy,
                lastLocation.Confidence,
                lastLocation.EventTimestamp,
                lastLocation.RawEventId,
                anchorCount);

            await _currentLocationRepository.UpdateAsync(currentLocation, ct);
        }

        await _unitOfWork.SaveChangesAsync(ct);
        return Result<Guid>.Success(currentLocation.Id);
    }
}