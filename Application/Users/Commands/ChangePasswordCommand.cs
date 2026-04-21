using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands;

public record ChangePasswordCommand(
    Guid UserId,
    string CurrentPassword,
    string NewPassword,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IPasswordHasher _hasher;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public ChangePasswordHandler(IUserRepository users, IPasswordHasher hasher, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _users = users;
        _hasher = hasher;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(ChangePasswordCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<Unit>.Failure("User not found");

        if (!_hasher.Verify(request.CurrentPassword, user.PasswordHash))
            return Result<Unit>.Failure("Invalid current password");

        user.ChangePassword(_hasher.Hash(request.NewPassword));
        await _users.UpdateAsync(user, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "password_change",
            "User",
            user.Id,
            null,
            "User password changed"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
