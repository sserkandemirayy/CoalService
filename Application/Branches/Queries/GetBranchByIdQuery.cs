using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Branches.Queries;

public record GetBranchByIdQuery(Guid Id, Guid PerformedByUserId) : IRequest<Result<Branch>>;

public class GetBranchByIdHandler : IRequestHandler<GetBranchByIdQuery, Result<Branch>>
{
    private readonly IBranchRepository _branches;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetBranchByIdHandler(IBranchRepository branches, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _branches = branches;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<Branch>> Handle(GetBranchByIdQuery request, CancellationToken ct)
    {
        var branch = await _branches.GetByIdAsync(request.Id, ct);
        if (branch is null)
            return Result<Branch>.Failure("Branch not found");

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "branch_view",
            "Branch",
            branch.Id,
            null,
            $"Branch '{branch.Name}' viewed"
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Branch>.Success(branch);
    }
}
