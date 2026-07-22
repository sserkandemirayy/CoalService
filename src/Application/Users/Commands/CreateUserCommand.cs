using Application.Common.Models;
using Domain.Abstractions;
using Domain.Constants;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Users.Commands;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    Guid UserTypeId,
    Guid CompanyId,
    string? Phone,
    string? Address,
    string? NationalId,
    string? Gender,
    DateTime? BirthDate,
    Guid? UserSpecializationId,
    Guid PerformedByUserId
) : IRequest<Result<Guid>>;

public class CreateUserValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.FirstName)
            .NotEmpty();

        RuleFor(x => x.LastName)
            .NotEmpty();

        RuleFor(x => x.UserTypeId)
            .NotEmpty();

        RuleFor(x => x.CompanyId)
            .NotEmpty();
    }
}

public class CreateUserHandler
    : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _users;
    private readonly IUserTypeRepository _userTypes;
    private readonly IUserSpecializationRepository _specializations;
    private readonly IRoleRepository _roles;
    private readonly ICompanyRepository _companies;
    private readonly IUserCompanyRepository _userCompanies;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public CreateUserHandler(
        IUserRepository users,
        IUserTypeRepository userTypes,
        IUserSpecializationRepository specializations,
        IRoleRepository roles,
        ICompanyRepository companies,
        IUserCompanyRepository userCompanies,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _userTypes = userTypes;
        _specializations = specializations;
        _roles = roles;
        _companies = companies;
        _userCompanies = userCompanies;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(
        CreateUserCommand req,
        CancellationToken ct)
    {
        var email = req.Email.Trim().ToLowerInvariant();

        var existing = await _users.FindByEmailAsync(email, ct);

        if (existing is not null)
            return Result<Guid>.Failure("Email already exists");

        var type = await _userTypes.GetByIdAsync(
            req.UserTypeId,
            ct);

        if (type is null)
            return Result<Guid>.Failure("Invalid UserTypeId");

        var company = await _companies.GetByIdAsync(
            req.CompanyId,
            ct);

        if (company is null)
            return Result<Guid>.Failure(
                "Company not found or you do not have access to it");

        if (req.UserSpecializationId.HasValue)
        {
            var specialization =
                await _specializations.GetByIdAsync(
                    req.UserSpecializationId.Value,
                    ct);

            if (specialization is null)
            {
                return Result<Guid>.Failure(
                    "Specialization not found");
            }

            if (specialization.UserTypeId != req.UserTypeId)
            {
                return Result<Guid>.Failure(
                    "Specialization does not belong to selected UserType");
            }
        }

        var defaultRole = await _roles.FindByNameAsync(
            RtlsRoleNames.Viewer,
            ct);

        if (defaultRole is null)
        {
            return Result<Guid>.Failure(
                $"Default role '{RtlsRoleNames.Viewer}' is not configured");
        }

        const string password = "123456";

        var user = User.Create(
            email,
            _hasher.Hash(password),
            req.FirstName.Trim(),
            req.LastName.Trim(),
            req.UserTypeId);

        user.CreatedBy = req.PerformedByUserId;

        user.UpdateProfile(
            req.FirstName.Trim(),
            req.LastName.Trim(),
            req.Phone ?? string.Empty,
            req.Address ?? string.Empty,
            req.NationalId ?? string.Empty,
            req.Gender ?? string.Empty);

        user.SetBirthDate(req.BirthDate);

        if (req.UserSpecializationId.HasValue)
            user.SetSpecialization(req.UserSpecializationId.Value);

        await _users.AddAsync(user, ct);

        var userCode = await _userCompanies.AddOrReactivateAsync(
            user.Id,
            req.CompanyId,
            ct);

        if (string.IsNullOrWhiteSpace(userCode))
        {
            return Result<Guid>.Failure(
                "User code could not be generated for the selected company");
        }

        await _users.AssignRoleAsync(
            user,
            defaultRole,
            ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(user.Id);
    }
}