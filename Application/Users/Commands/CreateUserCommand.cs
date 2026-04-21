using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System.Reflection;

namespace Application.Users.Commands;

public record CreateUserCommand(
    string Email,
    string FirstName,
    string LastName,
    Guid UserTypeId,
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
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.FirstName).NotEmpty();
        RuleFor(x => x.LastName).NotEmpty();
        RuleFor(x => x.UserTypeId).NotEmpty();
    }
}

public class CreateUserHandler : IRequestHandler<CreateUserCommand, Result<Guid>>
{
    private readonly IUserRepository _users;
    private readonly IUserTypeRepository _userTypes;
    private readonly IUserSpecializationRepository _specializations;
    private readonly IRoleRepository _roles;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;

    public CreateUserHandler(
        IUserRepository users,
        IUserTypeRepository userTypes,
        IUserSpecializationRepository specializations,
        IRoleRepository roles,
        IPasswordHasher hasher,
        IUnitOfWork uow)
    {
        _users = users;
        _userTypes = userTypes;
        _specializations = specializations;
        _roles = roles;
        _hasher = hasher;
        _uow = uow;
    }

    public async Task<Result<Guid>> Handle(CreateUserCommand req, CancellationToken ct)
    {
        // Email kontrolü
        var existing = await _users.FindByEmailAsync(req.Email, ct);
        if (existing is not null)
            return Result<Guid>.Failure("Email already exists");

        var type = await _userTypes.GetByIdAsync(req.UserTypeId, ct);
        if (type == null)
            return Result<Guid>.Failure("Invalid UserTypeId");

        // === Specialization kontrolü ===
        if (req.UserSpecializationId.HasValue)
        {
            var spec = await _specializations.GetByIdAsync(req.UserSpecializationId.Value, ct);
            if (spec == null)
                return Result<Guid>.Failure("Specialization not found");

            if (spec.UserTypeId != req.UserTypeId)
                return Result<Guid>.Failure("Specialization does not belong to selected UserType");
        }

        // === Şifre atama (şimdilik sabit 123456) ===
        var password = "123456";

        // === Strong password generator (ileride açacaksın) ===
        /*
        var password = PasswordGenerator.Generate(
            length: 12,
            includeUppercase: true,
            includeLowercase: true,
            includeDigits: true,
            includeSymbols: true
        );
        */

        var user = User.Create(
            req.Email,
            _hasher.Hash(password),
            req.FirstName,
            req.LastName,
            req.UserTypeId
        );

        // PII update
        user.UpdateProfile(
            req.FirstName,
            req.LastName,
            req.Phone ?? "",
            req.Address ?? "",
            req.NationalId ?? "",
            req.Gender ?? ""
        );

        if (req.BirthDate.HasValue)
        {
            var birthProp = typeof(User).GetProperty("BirthDate",
               BindingFlags.Instance |
               BindingFlags.NonPublic |
               BindingFlags.Public);

            birthProp?.SetValue(user, req.BirthDate.Value);
        }

        if (req.UserSpecializationId.HasValue)
            user.SetSpecialization(req.UserSpecializationId.Value);

        // === Rol atama ===
        var roleCode = type.Code.ToLowerInvariant() switch
        {
            "staff" => "staff",
            "doctor" => "doctor",
            "user" => "user",
            "patient" => "patient",
            _ => "user"
        };

        var role = await _roles.FindByNameAsync(roleCode, ct);
        if (role != null)
            user.AssignRole(role);

        await _users.AddAsync(user, ct);
        await _uow.SaveChangesAsync(ct);

        return Result<Guid>.Success(user.Id);
    }
}