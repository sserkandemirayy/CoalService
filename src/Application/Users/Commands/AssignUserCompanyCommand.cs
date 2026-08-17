using Application.Common.Models;
using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands;

public record AssignUserToCompanyCommand(
    Guid UserId,
    Guid CompanyId,
    Guid PerformedByUserId,
    string? UserCode = null)
    : IRequest<Result<Unit>>;

public class AssignUserToCompanyHandler
    : IRequestHandler<AssignUserToCompanyCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly ICompanyRepository _companies;
    private readonly IUserCompanyRepository _userCompanies;
    private readonly IUnitOfWork _uow;

    public AssignUserToCompanyHandler(
        IUserRepository users,
        ICompanyRepository companies,
        IUserCompanyRepository userCompanies,
        IUnitOfWork uow)
    {
        _users = users;
        _companies = companies;
        _userCompanies = userCompanies;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(
        AssignUserToCompanyCommand req,
        CancellationToken ct)
    {
        var user =
            await _users.GetByIdAsync(
                req.UserId,
                ct);

        if (user is null)
        {
            return Result<Unit>.Failure(
                "User not found");
        }

        var company =
            await _companies.GetByIdAsync(
                req.CompanyId,
                ct);

        if (company is null)
        {
            return Result<Unit>.Failure(
                "Company not found or you do not have access to it");
        }

        string? userCode;

        try
        {
            userCode =
                await _userCompanies
                    .AddOrReactivateAsync(
                        req.UserId,
                        req.CompanyId,
                        req.UserCode,
                        ct);
        }
        catch (InvalidOperationException ex)
        {
            return Result<Unit>.Failure(
                ex.Message);
        }

        if (string.IsNullOrWhiteSpace(userCode))
        {
            return Result<Unit>.Failure(
                "User could not be assigned to company");
        }

        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(
            Unit.Value);
    }
}