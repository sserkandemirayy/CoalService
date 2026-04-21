using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

public record ActivateUserCommand(Guid UserId, Guid PerformedByUserId) : IRequest<Result<Unit>>;

public class ActivateUserHandler : IRequestHandler<ActivateUserCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public ActivateUserHandler(IUserRepository users, IUnitOfWork uow, IAuditLogRepository audit)
    {
        _users = users;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(ActivateUserCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        if (user is null)
            return Result<Unit>.Failure("User not found");

        user.Activate();
        await _users.UpdateAsync(user, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "user_activated",
            "User",
            user.Id,
            null,
            $"User {user.Email} activated"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}
