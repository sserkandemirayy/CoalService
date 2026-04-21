using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record ActivateAnchorCommand(Guid Id, Guid PerformedByUserId) : IRequest<Result<Guid>>;

public sealed class ActivateAnchorCommandHandler : IRequestHandler<ActivateAnchorCommand, Result<Guid>>
{
    private readonly IAnchorRepository _anchorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ActivateAnchorCommandHandler(IAnchorRepository anchorRepository, IUnitOfWork unitOfWork)
    {
        _anchorRepository = anchorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(ActivateAnchorCommand request, CancellationToken ct)
    {
        var anchor = await _anchorRepository.GetByIdAsync(request.Id, ct);
        if (anchor is null)
            return Result<Guid>.Failure("Anchor not found.");

        anchor.Activate();
        anchor.UpdateAudit(request.PerformedByUserId);

        await _anchorRepository.UpdateAsync(anchor, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(anchor.Id);
    }
}