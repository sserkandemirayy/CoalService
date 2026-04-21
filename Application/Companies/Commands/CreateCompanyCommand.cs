using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Companies.Commands;

public record CreateCompanyCommand(
    string Name,
    string TaxNumber,
    string Address,
    string Phone,
    string Email,
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
) : IRequest<Result<Guid>>;

public class CreateCompanyValidator : AbstractValidator<CreateCompanyCommand>
{
    public CreateCompanyValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TaxNumber).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Phone).MaximumLength(50);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Website).MaximumLength(200).When(x => !string.IsNullOrEmpty(x.Website));
        RuleFor(x => x.LogoUrl).MaximumLength(300);
    }
}

public class CreateCompanyHandler : IRequestHandler<CreateCompanyCommand, Result<Guid>>
{
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public CreateCompanyHandler(ICompanyRepository companies, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _companies = companies;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Guid>> Handle(CreateCompanyCommand request, CancellationToken ct)
    {
        var company = Company.Create(
            request.Name,
            request.TaxNumber,
            request.Address,
            request.Phone,
            request.Email,
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

        await _companies.AddAsync(company, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "company_create",
            "Company",
            company.Id,
            null,
            $"Company '{company.Name}' created"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(company.Id);
    }
}