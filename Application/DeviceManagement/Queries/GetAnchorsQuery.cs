using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using Domain.Enums;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetAnchorsQuery(
    string? Search,
    string? Status,
    Guid? CompanyId,
    Guid? BranchId,
    int Page = 1,
    int PageSize = 20
) : IRequest<Result<IReadOnlyList<AnchorDto>>>;

public sealed class GetAnchorsQueryHandler : IRequestHandler<GetAnchorsQuery, Result<IReadOnlyList<AnchorDto>>>
{
    private readonly IAnchorRepository _anchorRepository;

    public GetAnchorsQueryHandler(IAnchorRepository anchorRepository)
    {
        _anchorRepository = anchorRepository;
    }

    public async Task<Result<IReadOnlyList<AnchorDto>>> Handle(GetAnchorsQuery request, CancellationToken ct)
    {
        AnchorStatus? status = null;

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!Enum.TryParse<AnchorStatus>(request.Status, true, out var parsedStatus))
                return Result<IReadOnlyList<AnchorDto>>.Failure("Invalid anchor status.");

            status = parsedStatus;
        }

        var (items, _) = await _anchorRepository.GetPagedAsync(
            request.Search,
            status,
            request.CompanyId,
            request.BranchId,
            request.Page,
            request.PageSize,
            ct);

        var dto = items
            .Select(x => new AnchorDto(
                x.Id,
                x.ExternalId,
                x.Code,
                x.Name,
                x.IpAddress,
                x.Status.ToString(),
                x.IsActive,
                x.LastHeartbeatAt,
                x.LastStatusChangedAt,
                x.CompanyId,
                x.BranchId,
                x.MetadataJson))
            .ToList();

        return Result<IReadOnlyList<AnchorDto>>.Success(dto);
    }
}