using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Commands;

public record DeactivateUserCommand(Guid UserId, Guid PerformedByUserId) : IRequest<Result<Unit>>;

public class DeactivateUserHandler : IRequestHandler<DeactivateUserCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public DeactivateUserHandler(
        IUserRepository users,
        IUnitOfWork uow,
        IAuditLogRepository audit)
    {
        _users = users;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(DeactivateUserCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdSingleAsync(request.UserId, ct);

        if (user is null)
            return Result<Unit>.Failure("User not found");

        if (!user.IsActive)
            return Result<Unit>.Success(Unit.Value);

        user.Deactivate();

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "user_deactivated",
            "User",
            user.Id,
            null,
            $"User {user.Email} deactivated"
        ), ct);

        await _uow.SaveChangesAsync(ct);

        return Result<Unit>.Success(Unit.Value);
    }
}
