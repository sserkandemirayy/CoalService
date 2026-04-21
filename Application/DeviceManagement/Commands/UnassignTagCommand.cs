using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.DeviceManagment.Commands;

public sealed record UnassignTagCommand(Guid TagId, Guid PerformedByUserId) : IRequest<Result<Guid>>;

public sealed class UnassignTagCommandHandler : IRequestHandler<UnassignTagCommand, Result<Guid>>
{
    private readonly ITagAssignmentRepository _tagAssignmentRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UnassignTagCommandHandler(ITagAssignmentRepository tagAssignmentRepository, IUnitOfWork unitOfWork)
    {
        _tagAssignmentRepository = tagAssignmentRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<Guid>> Handle(UnassignTagCommand request, CancellationToken ct)
    {
        var assignment = await _tagAssignmentRepository.GetActiveByTagIdAsync(request.TagId, ct);
        if (assignment is null)
            return Result<Guid>.Failure("Active assignment not found.");

        assignment.Unassign();
        assignment.UpdateAudit(request.PerformedByUserId);

        await _tagAssignmentRepository.UpdateAsync(assignment, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return Result<Guid>.Success(assignment.Id);
    }
}