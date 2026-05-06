using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Branches.Commands;

public record DeleteBranchCommand(Guid Id, Guid PerformedByUserId) : IRequest<Result<Unit>>;

public class DeleteBranchHandler : IRequestHandler<DeleteBranchCommand, Result<Unit>>
{
    private readonly IBranchRepository _branches;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public DeleteBranchHandler(IBranchRepository branches, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _branches = branches;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(DeleteBranchCommand request, CancellationToken ct)
    {
        var branch = await _branches.GetByIdAsync(request.Id, ct);
        if (branch is null)
            return Result<Unit>.Failure("Branch not found");

        branch.SoftDelete(request.PerformedByUserId);
        await _branches.UpdateAsync(branch, ct);

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "branch_delete",
            "Branch",
            branch.Id,
            null,
            $"Branch '{branch.Name}' soft-deleted"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }

}
