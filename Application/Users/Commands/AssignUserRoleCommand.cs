using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

public record AssignUserRoleCommand(Guid UserId, Guid RoleId, Guid PerformedByUserId)
    : IRequest<Result<Unit>>;

public class AssignUserRoleHandler : IRequestHandler<AssignUserRoleCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUnitOfWork _uow;
    private readonly IAuditLogRepository _audit;

    public AssignUserRoleHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUnitOfWork uow,
        IAuditLogRepository audit)
    {
        _users = users;
        _roles = roles;
        _uow = uow;
        _audit = audit;
    }

    public async Task<Result<Unit>> Handle(AssignUserRoleCommand request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.UserId, ct);
        var role = await _roles.GetByIdAsync(request.RoleId, ct);

        if (user is null || role is null)
            return Result<Unit>.Failure("User or Role not found");

        // Repository EF Core kullanarak IgnoreQueryFilters ile kontrol edecek
        await _users.AssignRoleAsync(user, role, ct);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "user_role_assigned",
            "User",
            user.Id,
            null,
            $"Role '{role.Name}' assigned to {user.Email}"
        ), ct);

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}