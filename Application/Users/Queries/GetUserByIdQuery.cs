using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;

namespace Application.Users.Queries;

public record GetUserByIdQuery(Guid Id, Guid PerformedByUserId) : IRequest<Result<UserDetailedDto>>;

public class GetUserByIdHandler : IRequestHandler<GetUserByIdQuery, Result<UserDetailedDto>>
{
    private readonly IUserRepository _users;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetUserByIdHandler(IUserRepository users, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _users = users;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<UserDetailedDto>> Handle(GetUserByIdQuery request, CancellationToken ct)
    {
        var user = await _users.GetByIdAsync(request.Id, ct);

        if (user is null)
            return Result<UserDetailedDto>.Failure("User not found");

        // view_pii izni olanlar tam görsün, diğerleri masked
        var canViewPII = await _users.UserHasPermissionAsync(request.PerformedByUserId, "view_pii", ct);

        var dto = UserDetailedDto.FromEntity(user, canViewPII);

        await _audit.AddAsync(AuditLog.Create(
            request.PerformedByUserId,
            "user_view",
            "User",
            user.Id,
            null,
            $"User {user.Email} viewed"
        ), ct);
        await _uow.SaveChangesAsync(ct);

        return Result<UserDetailedDto>.Success(dto);
    }
}
