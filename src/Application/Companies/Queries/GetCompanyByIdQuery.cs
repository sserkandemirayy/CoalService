using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Companies.Queries;

public record GetCompanyByIdQuery(Guid Id, Guid PerformedByUserId) : IRequest<Result<Company>>;

public class GetCompanyByIdHandler : IRequestHandler<GetCompanyByIdQuery, Result<Company>>
{
    private readonly ICompanyRepository _companies;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetCompanyByIdHandler(ICompanyRepository companies, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _companies = companies;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<Company>> Handle(GetCompanyByIdQuery request, CancellationToken ct)
    {
        var company = await _companies.GetByIdAsync(request.Id, ct);
        if (company is null)
            return Result<Company>.Failure("Company not found");

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "company_view",
            "Company",
            company.Id,
            null,
            $"Company '{company.Name}' viewed"
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Company>.Success(company);
    }
}
