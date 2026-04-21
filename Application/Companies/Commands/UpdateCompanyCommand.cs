using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Companies.Commands;

public record UpdateCompanyCommand(
    Guid Id,
    string Name,
    string Address,
    string Phone,
    string Email,
    string TaxNumber, 
    string? Title,
    string? TaxOffice,
    string? City,
    string? District,
    string? PostalCode,
    string? Website,
    string? LogoUrl,
    string? WorkHours,
    string? Holidays,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class UpdateCompanyValidator : AbstractValidator<UpdateCompanyCommand>
{
    public UpdateCompanyValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.TaxNumber).NotEmpty().MaximumLength(50); 
        RuleFor(x => x.Website).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Website));
        RuleFor(x => x.LogoUrl).MaximumLength(300);
    }
}

public class UpdateCompanyHandler : IRequestHandler<UpdateCompanyCommand, Result<Unit>>
{
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public UpdateCompanyHandler(ICompanyRepository companies, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _companies = companies;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(UpdateCompanyCommand request, CancellationToken ct)
    {
        var company = await _companies.GetByIdAsync(request.Id, ct);
        if (company is null)
            return Result<Unit>.Failure("Company not found");

        company.Update(
            request.Name,
            request.Address,
            request.Phone,
            request.Email,
            request.TaxNumber,
            request.Title,
            request.TaxOffice,
            request.City,
            request.District,
            request.PostalCode,
            request.Website,
            request.LogoUrl,
            request.WorkHours,
            request.Holidays
        );

        await _companies.UpdateAsync(company, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "company_update",
            "Company",
            company.Id,
            null,
            $"Company '{company.Name}' updated"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}