using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Auth.Commands;

public record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName
) : IRequest<Result<AuthResponse>>;

public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MaximumLength(100);
    }
}

public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
{
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IDateTimeProvider _clock;
    private readonly IUnitOfWork _uow;
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUserTypeRepository _userTypes;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IAuditLogRepository _audit;

    public RegisterUserHandler(
        IPasswordHasher hasher,
        IJwtTokenGenerator jwt,
        IDateTimeProvider clock,
        IUnitOfWork uow,
        IUserRepository users,
        IRoleRepository roles,
        IUserTypeRepository userTypes,
        IRefreshTokenRepository refreshRepo,
        IAuditLogRepository audit)
    {
        _hasher = hasher;
        _jwt = jwt;
        _clock = clock;
        _uow = uow;
        _users = users;
        _roles = roles;
        _userTypes = userTypes;
        _refreshRepo = refreshRepo;
        _audit = audit;
    }

    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
    {
        // Email kontrolü
        var existing = await _users.FindByEmailAsync(request.Email, ct);
        if (existing is not null)
            return Result<AuthResponse>.Failure("Email already exists");

        // Default user type al ("user" koduyla belirlenecek)
        var userType = await _userTypes.FindByCodeAsync("user", ct);
        if (userType is null)
            return Result<AuthResponse>.Failure("System user type 'user' is not configured");

        // Yeni user oluþtur
        var user = User.Create(
            request.Email,
            _hasher.Hash(request.Password),
            request.FirstName,
            request.LastName,
            userType.Id
        );

        await _users.AddAsync(user, ct);

        // Default rol ata ("user")
        var role = await _roles.FindByNameAsync("user", ct);
        if (role == null)
            return Result<AuthResponse>.Failure("Default role 'user' is not configured");

        await _users.AssignRoleAsync(user, role, ct);

        // JWT token
        var roles = await _users.GetRoleNamesAsync(user.Id, ct);
        var (access, exp) = _jwt.CreateAccessToken(user, roles);
        var refresh = _jwt.GenerateRefreshToken();

        await _refreshRepo.SaveAsync(user.Id, refresh, _clock.UtcNow, null, null, null, ct);

        // Audit
        await _audit.AddAsync(
            AuditLog.Create(
                user.Id,
                "user_registered",
                "User",
                user.Id,
                null,
                "Self registered user (default user type & role)"
            ), ct);

        await _uow.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(access, exp, refresh));
    }
}



//using Domain.Abstractions;
//using Application.Common.Models;
//using FluentValidation;
//using MediatR;
//using Domain.Entities;

//namespace Application.Auth.Commands;

//public record RegisterUserCommand(
//    string Email,
//    string? Password,           // Opsiyonel
//    string FirstName,
//    string LastName,
//    Guid RoleId,
//    Guid UserTypeId,           
//    string? PhoneEncrypted = null,
//    string? Gender = null,
//    string? AddressEncrypted = null,
//    string? NationalIdEncrypted = null,
//    DateTime? BirthDate = null,
//    Guid? UserSpecializationId = null
//) : IRequest<Result<AuthResponse>>;

//public class RegisterUserValidator : AbstractValidator<RegisterUserCommand>
//{
//    public RegisterUserValidator()
//    {
//        RuleFor(x => x.Email)
//            .NotEmpty()
//            .EmailAddress();

//        RuleFor(x => x.FirstName)
//            .NotEmpty()
//            .MaximumLength(100);

//        RuleFor(x => x.LastName)
//            .NotEmpty()
//            .MaximumLength(100);

//        RuleFor(x => x.RoleId)
//            .NotEmpty()
//            .WithMessage("RoleId is required");

//        RuleFor(x => x.UserTypeId)
//            .NotEmpty()
//            .WithMessage("UserTypeId is required"); 

//        When(x => !string.IsNullOrWhiteSpace(x.Password), () =>
//        {
//            RuleFor(x => x.Password)
//                .MinimumLength(6);
//        });
//    }
//}

//public class RegisterUserHandler : IRequestHandler<RegisterUserCommand, Result<AuthResponse>>
//{
//    private readonly IPasswordHasher _hasher;
//    private readonly IJwtTokenGenerator _jwt;
//    private readonly IDateTimeProvider _clock;
//    private readonly IUnitOfWork _uow;
//    private readonly IUserRepository _users;
//    private readonly IRoleRepository _roles;
//    private readonly IUserTypeRepository _userTypes;
//    private readonly IUserSpecializationRepository _specializations;
//    private readonly IRefreshTokenRepository _refreshRepo;
//    private readonly IAuditLogRepository _audit;

//    public RegisterUserHandler(
//        IPasswordHasher hasher,
//        IJwtTokenGenerator jwt,
//        IDateTimeProvider clock,
//        IUnitOfWork uow,
//        IUserRepository users,
//        IRoleRepository roles,
//        IUserTypeRepository userTypes,
//        IUserSpecializationRepository userSpecializations,
//        IRefreshTokenRepository refreshRepo,
//        IAuditLogRepository audit)
//    {
//        _hasher = hasher;
//        _jwt = jwt;
//        _clock = clock;
//        _uow = uow;
//        _users = users;
//        _roles = roles;
//        _userTypes = userTypes;
//        _refreshRepo = refreshRepo;
//        _audit = audit;
//        _specializations = userSpecializations;
//    }

//    public async Task<Result<AuthResponse>> Handle(RegisterUserCommand request, CancellationToken ct)
//    {
//        // Email kontrolü
//        var existing = await _users.FindByEmailAsync(request.Email, ct);
//        if (existing is not null)
//            return Result<AuthResponse>.Failure("Email already exists");

//        // UserType kontrolü 
//        var userType = await _userTypes.GetByIdAsync(request.UserTypeId, ct);
//        if (userType is null)
//            return Result<AuthResponse>.Failure("Invalid UserTypeId");

//        // Þifre bossa varsayýlan ata
//        var passwordToUse = string.IsNullOrWhiteSpace(request.Password)
//            ? "123456"
//            : request.Password;

//        // Yeni kullanýcý oluþtur
//        var user = User.Create(
//            request.Email,
//            _hasher.Hash(passwordToUse),
//            request.FirstName,
//            request.LastName,
//            request.UserTypeId 
//        );

//        // Opsiyonel alanlar
//        if (!string.IsNullOrWhiteSpace(request.PhoneEncrypted))
//            user.UpdateProfile(user.FirstName, user.LastName, request.PhoneEncrypted, request.AddressEncrypted ?? "", "", "");

//        if (!string.IsNullOrWhiteSpace(request.Gender))
//        {
//            var genderField = typeof(User).GetProperty("Gender", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//            genderField?.SetValue(user, request.Gender);
//        }

//        if (!string.IsNullOrWhiteSpace(request.NationalIdEncrypted))
//        {
//            var nationalIdField = typeof(User).GetProperty("NationalIdEncrypted", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//            nationalIdField?.SetValue(user, request.NationalIdEncrypted);
//        }

//        if (request.BirthDate.HasValue)
//        {
//            var birthDateField = typeof(User).GetProperty("BirthDate", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public);
//            birthDateField?.SetValue(user, request.BirthDate.Value);
//        }

//        // === Specialization (opsiyonel) ===
//        if (request.UserSpecializationId.HasValue)
//        {
//            var spec = await _specializations.GetByIdAsync(request.UserSpecializationId.Value, ct);
//            if (spec == null)
//                return Result<AuthResponse>.Failure("Specialization not found");

//            // specialization seçilen userType'a ait olmalý
//            if (spec.UserTypeId != request.UserTypeId)
//                return Result<AuthResponse>.Failure("Specialization does not belong to the selected UserType");

//            user.SetSpecialization(spec.Id);
//        }

//        await _users.AddAsync(user, ct);

//        // RoleId üzerinden rol ata
//        var role = await _roles.GetByIdAsync(request.RoleId, ct);
//        if (role is null)
//            return Result<AuthResponse>.Failure($"Role with ID '{request.RoleId}' not found");

//        await _users.AssignRoleAsync(user, role, ct);

//        // Token oluþtur
//        var roles = await _users.GetRoleNamesAsync(user.Id, ct);
//        var (access, exp) = _jwt.CreateAccessToken(user, roles);
//        var refresh = _jwt.GenerateRefreshToken();

//        await _refreshRepo.SaveAsync(user.Id, refresh, _clock.UtcNow, null, null, null, ct);

//        // Audit log kaydý
//        await _audit.AddAsync(AuditLog.Create(user.Id, "user_registered", "User", user.Id, null,
//            $"New user registered (Type={userType.Name})"), ct);

//        await _uow.SaveChangesAsync(ct);

//        return Result<AuthResponse>.Success(new AuthResponse(access, exp, refresh));
//    }
//}

