using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Branches.Queries;

public record GetBranchesByCompanyIdQuery(Guid CompanyId, Guid PerformedByUserId) : IRequest<Result<IEnumerable<Branch>>>;

public class GetBranchesByCompanyIdHandler : IRequestHandler<GetBranchesByCompanyIdQuery, Result<IEnumerable<Branch>>>
{
    private readonly IBranchRepository _branches;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetBranchesByCompanyIdHandler(IBranchRepository branches, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _branches = branches;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<IEnumerable<Branch>>> Handle(GetBranchesByCompanyIdQuery request, CancellationToken ct)
    {
        var branches = await _branches.GetByCompanyIdAsync(request.CompanyId, ct);

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "branch_list",
            "Branch",
            null,
            null,
            $"Branches listed for Company {request.CompanyId}"
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<IEnumerable<Branch>>.Success(branches);
    }
}
