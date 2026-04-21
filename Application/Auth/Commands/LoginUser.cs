using Domain.Abstractions;
using Application.Common.Models;
using FluentValidation;
using MediatR;
using Domain.Entities;

namespace Application.Auth.Commands;

public record LoginUserCommand(string Email, string Password) : IRequest<Result<AuthResponse>>;

public class LoginUserValidator : AbstractValidator<LoginUserCommand>
{
    public LoginUserValidator()
    {
        RuleFor(x => x.Email).NotEmpty().EmailAddress();
        RuleFor(x => x.Password).NotEmpty();
    }
}

public class LoginUserHandler : IRequestHandler<LoginUserCommand, Result<AuthResponse>>
{
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;
    private readonly IDateTimeProvider _clock;
    private readonly IUserRepository _users;
    private readonly IRefreshTokenRepository _refreshRepo;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit; // yeni

    public LoginUserHandler(
        IPasswordHasher hasher,
        IJwtTokenGenerator jwt,
        IDateTimeProvider clock,
        IUserRepository users,
        IRefreshTokenRepository refreshRepo,
        IUnitOfWork uow,
        IAuditLogRepository audit) // ekledik
    {
        _hasher = hasher;
        _jwt = jwt;
        _clock = clock;
        _users = users;
        _refreshRepo = refreshRepo;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<AuthResponse>> Handle(LoginUserCommand request, CancellationToken ct)
    {
        var user = await _users.FindByEmailAsync(request.Email, ct);
        if (user is null)
            return Result<AuthResponse>.Failure("Invalid credentials");

        if (!user.IsActive)
            return Result<AuthResponse>.Failure("Account is inactive");

        if (user.LockoutEnd.HasValue && user.LockoutEnd.Value > _clock.UtcNow)
            return Result<AuthResponse>.Failure($"Account locked until {user.LockoutEnd.Value:u}");

        if (!_hasher.Verify(request.Password, user.PasswordHash))
        {
            await _users.RecordLoginFailureAsync(user, 5, TimeSpan.FromMinutes(15), ct);
            await _uow.SaveChangesAsync(ct);

            // baþarýsýz login giriþimlerini de loglayabiliriz
            await _audit.AddAsync(AuditLog.Create(user.Id, "login_failed", "User", user.Id, null, "Invalid password"), ct);
            await _uow.SaveChangesAsync(ct);

            return Result<AuthResponse>.Failure("Invalid credentials");
        }

        await _users.RecordLoginSuccessAsync(user, ct);

        var roles = await _users.GetRoleNamesAsync(user.Id, ct);
        var (access, exp) = _jwt.CreateAccessToken(user, roles);
        var refresh = _jwt.GenerateRefreshToken();
        await _refreshRepo.SaveAsync(user.Id, refresh, _clock.UtcNow, null, null,null, ct);

        // baþarýlý login logu
        await _audit.AddAsync(AuditLog.Create(user.Id, "login_success", "User", user.Id, null, "User logged in"), ct);

        await _uow.SaveChangesAsync(ct);

        return Result<AuthResponse>.Success(new AuthResponse(access, exp, refresh));
    }
}
