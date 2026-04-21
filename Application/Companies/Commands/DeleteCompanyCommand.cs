using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Companies.Commands;

public record DeleteCompanyCommand(Guid Id, Guid PerformedByUserId) : IRequest<Result<Unit>>;

public class DeleteCompanyHandler : IRequestHandler<DeleteCompanyCommand, Result<Unit>>
{
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public DeleteCompanyHandler(ICompanyRepository companies, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _companies = companies;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(DeleteCompanyCommand request, CancellationToken ct)
    {
        var company = await _companies.GetByIdAsync(request.Id, ct);
        if (company is null)
            return Result<Unit>.Failure("Company not found");

        company.SoftDelete(request.PerformedByUserId);
        await _companies.UpdateAsync(company, ct);

        // Audit log
        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "company_delete",
            "Company",
            company.Id,
            null,
            $"Company '{company.Name}' soft-deleted"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }

}
