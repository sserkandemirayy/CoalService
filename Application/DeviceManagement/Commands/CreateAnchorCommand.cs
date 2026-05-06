using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record CreateAnchorCommand(
    string ExternalId,
    string Code,
    string? Name,
    string? IpAddress,
    Guid? CompanyId,
    Guid? BranchId,
    string? MetadataJson,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public sealed class CreateAnchorCommandHandler : IRequestHandler<CreateAnchorCommand, Result<Guid>>
{
    private readonly IAnchorRepository _anchorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAnchorCommandHandler(IAnchorRepository anchorRepository, IUnitOfWork unitOfWork)
    {
        _anchorRepository = anchorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(CreateAnchorCommand request, CancellationToken ct)
    {
        if (await _anchorRepository.GetByExternalIdAsync(request.ExternalId, ct) is not null)
            return Result<Guid>.Failure("Anchor external id already exists.");

        if (await _anchorRepository.GetByCodeAsync(request.Code, ct) is not null)
            return Result<Guid>.Failure("Anchor code already exists.");

        var anchor = Anchor.Create(
            request.ExternalId,
            request.Code,
            request.Name,
            request.IpAddress,
            request.CompanyId,
            request.BranchId,
            request.MetadataJson);

        await _anchorRepository.AddAsync(anchor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(anchor.Id);
    }
}