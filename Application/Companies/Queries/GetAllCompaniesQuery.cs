using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Companies.Queries;

public record GetAllCompaniesQuery(Guid PerformedByUserId) : IRequest<Result<IEnumerable<Company>>>;

public class GetAllCompaniesHandler : IRequestHandler<GetAllCompaniesQuery, Result<IEnumerable<Company>>>
{
    private readonly ICompanyRepository _companies;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetAllCompaniesHandler(ICompanyRepository companies, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _companies = companies;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<IEnumerable<Company>>> Handle(GetAllCompaniesQuery request, CancellationToken ct)
    {
        var companies = await _companies.GetAllAsync(ct);

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "company_list",
            "Company",
            null,
            null,
            "All companies listed"
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<IEnumerable<Company>>.Success(companies);
    }
}
