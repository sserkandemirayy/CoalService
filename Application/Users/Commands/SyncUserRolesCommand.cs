using Application.Common.Models;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

public record SyncUserRolesCommand(
    Guid UserId,
    IEnumerable<string> Add,
    IEnumerable<string> Remove,
    Guid PerformedByUserId
) : IRequest<Result<Unit>>;

public class SyncUserRolesHandler : IRequestHandler<SyncUserRolesCommand, Result<Unit>>
{
    private readonly IUserRepository _users;
    private readonly IRoleRepository _roles;
    private readonly IUnitOfWork _uow;

    public SyncUserRolesHandler(
        IUserRepository users,
        IRoleRepository roles,
        IUnitOfWork uow)
    {
        _users = users;
        _roles = roles;
        _uow = uow;
    }

    public async Task<Result<Unit>> Handle(SyncUserRolesCommand req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null)
            return Result<Unit>.Failure("User not found");

     
        foreach (var addName in req.Add)
        {
            var role = await _roles.FindByNameAsync(addName, ct);
            if (role != null)
                await _users.AssignRoleAsync(user, role, ct); // repository içinde reactivation logic
        }

    
        foreach (var remName in req.Remove)
        {
            var role = await _roles.FindByNameAsync(remName, ct);
            if (role != null)
                user.RemoveRole(role); // Domain metodu, soft delete uygulanıyor
        }

        await _uow.SaveChangesAsync(ct);
        return Result<Unit>.Success(Unit.Value);
    }
}