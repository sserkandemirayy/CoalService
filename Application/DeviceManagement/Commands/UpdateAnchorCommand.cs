using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record UpdateAnchorCommand(
    Guid Id,
    string Code,
    string? Name,
    string? IpAddress,
    Guid? CompanyId,
    Guid? BranchId,
    string? MetadataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class UpdateAnchorCommandHandler : IRequestHandler<UpdateAnchorCommand, Result<Guid>>
{
    private readonly IAnchorRepository _anchorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateAnchorCommandHandler(IAnchorRepository anchorRepository, IUnitOfWork unitOfWork)
    {
        _anchorRepository = anchorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UpdateAnchorCommand request, CancellationToken ct)
    {
        var anchor = await _anchorRepository.GetByIdAsync(request.Id, ct);
        if (anchor is null)
            return Result<Guid>.Failure("Anchor not found.");

        var duplicate = await _anchorRepository.GetByCodeAsync(request.Code, ct);
        if (duplicate is not null && duplicate.Id != request.Id)
            return Result<Guid>.Failure("Anchor code already exists.");

        anchor.UpdateInfo(
            request.Code,
            request.Name,
            request.IpAddress,
            request.CompanyId,
            request.BranchId,
            request.MetadataJson);

        anchor.UpdateAudit(request.PerformedByUserId);

        await _anchorRepository.UpdateAsync(anchor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(anchor.Id);
    }
}