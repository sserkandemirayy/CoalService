using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Branches.Commands;

public record CreateBranchCommand(
    Guid CompanyId,
    string Name,
    string Address,
    string Phone,
    string Email,
    string? Code,
    string? City,
    string? District,
    string? PostalCode,
    string? Website,
    Guid? ManagerUserId,
    DateTime? OpenedAt,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public class CreateBranchValidator : AbstractValidator<CreateBranchCommand>
{
    public CreateBranchValidator()
    {
        RuleFor(x => x.CompanyId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(50);
    }
}

public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, Result<Guid>>
{
    private readonly IBranchRepository _branches;
    private readonly ICompanyRepository _companies;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public CreateBranchHandler(IBranchRepository branches, ICompanyRepository companies, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _branches = branches;
        _companies = companies;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Guid>> Handle(CreateBranchCommand request, CancellationToken ct)
    {
        var company = await _companies.GetByIdAsync(request.CompanyId, ct);
        if (company is null)
            return Result<Guid>.Failure("Company not found");

        var branch = Branch.Create(
            request.CompanyId,
            request.Name,
            request.Address,
            request.Phone,
            request.Email,
            request.Code,
            request.City,
            request.District,
            request.PostalCode,
            request.Website,
            request.ManagerUserId,
            request.OpenedAt
        );

        await _branches.AddAsync(branch, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "branch_create",
            "Branch",
            branch.Id,
            null,
            $"Branch '{branch.Name}' created under Company '{company.Name}'"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Guid>.Success(branch.Id);
    }
}