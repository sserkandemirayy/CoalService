using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

// === Request ===
public record GetMeQuery(Guid UserId) : IRequest<Result<UserDetailedDto>>;

// === Handler ===
public class GetMeHandler : IRequestHandler<GetMeQuery, Result<UserDetailedDto>>
{
    private readonly IUserRepository _users;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetMeHandler(IUserRepository users, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _users = users;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<UserDetailedDto>> Handle(GetMeQuery req, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(req.UserId, ct);
        if (user is null)
            return Result<UserDetailedDto>.Failure("User not found");

        // Roller & Ýzinler
        var roles = await _users.GetRoleNamesAsync(user.Id, ct);
        var permissions = await _users.GetPermissionNamesAsync(user.Id, ct);

        // PII yetkisi
        var canViewPII = permissions.Contains("view_pii");

        // DTO oluþtur
        var dto = UserDetailedDto.FromEntity(user, canViewPII) with
        {
            Roles = roles,
            Permissions = permissions
        };

        // Audit log (isteðe baðlý)
        if (canViewPII)
        {
            await _audit.AddAsync(AuditLog.Create(
                req.UserId,
                "view_pii",
                "User",
                user.Id,
                null,
                "User viewed own PII data"
            ), ct);
            await _uow.SaveChangesAsync(ct);
        }

        return Result<UserDetailedDto>.Success(dto);
    }
}
