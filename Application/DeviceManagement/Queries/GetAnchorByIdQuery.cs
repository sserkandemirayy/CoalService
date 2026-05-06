using Application.Common.Models;
using Application.DTOs.DeviceManagement;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Queries;

public sealed record GetAnchorByIdQuery(Guid Id) : IRequest<Result<AnchorDto>>;

public sealed class GetAnchorByIdQueryHandler : IRequestHandler<GetAnchorByIdQuery, Result<AnchorDto>>
{
    private readonly IAnchorRepository _anchorRepository;

    public GetAnchorByIdQueryHandler(IAnchorRepository anchorRepository)
    {
        _anchorRepository = anchorRepository;
    }

    public async Task<Result<AnchorDto>> Handle(GetAnchorByIdQuery request, CancellationToken ct)
    {
        var anchor = await _anchorRepository.GetByIdAsync(request.Id, ct);
        if (anchor is null)
            return Result<AnchorDto>.Failure("Anchor not found.");

        var dto = new AnchorDto(
            anchor.Id,
            anchor.ExternalId,
            anchor.Code,
            anchor.Name,
            anchor.IpAddress,
            anchor.Status.ToString(),
            anchor.IsActive,
            anchor.LastHeartbeatAt,
            anchor.LastStatusChangedAt,
            anchor.CompanyId,
            anchor.BranchId,
            anchor.MetadataJson);

        return Result<AnchorDto>.Success(dto);
    }
}