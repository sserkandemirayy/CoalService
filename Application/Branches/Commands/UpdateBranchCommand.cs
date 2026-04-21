using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Branches.Commands;

public record UpdateBranchCommand(
    Guid Id,
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
) : IRequest<Result<Unit>>;

public class UpdateBranchValidator : AbstractValidator<UpdateBranchCommand>
{
    public UpdateBranchValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Address).NotEmpty().MaximumLength(500);
        RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(150);
        RuleFor(x => x.Phone).MaximumLength(50);
    }
}

public class UpdateBranchHandler : IRequestHandler<UpdateBranchCommand, Result<Unit>>
{
    private readonly IBranchRepository _branches;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public UpdateBranchHandler(IBranchRepository branches, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _branches = branches;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(UpdateBranchCommand request, CancellationToken ct)
    {
        var branch = await _branches.GetByIdAsync(request.Id, ct);
        if (branch is null)
            return Result<Unit>.Failure("Branch not found");

        branch.Update(
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

        await _branches.UpdateAsync(branch, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "branch_update",
            "Branch",
            branch.Id,
            null,
            $"Branch '{branch.Name}' updated"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}