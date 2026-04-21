using Application.Common.Models;
using Application.DTOs.Users;
using Domain.Abstractions;
using Domain.Entities;
using MediatR;
using System.Linq;

namespace Application.Users.Queries;

// === Request ===
public record GetAllUsersQuery(
    Guid PerformedByUserId,
    string? Q = null,
    string? Role = null,
    string? Status = null,
    List<string>? UserTypeCodes = null,
    Guid? SpecializationId = null,
    int Page = 1,
    int PageSize = 20,
    string? Sort = "FirstName"
) : IRequest<Result<PagedResult<UserDetailedDto>>>;

// === Handler ===
// === Handler ===
public class GetAllUsersHandler : IRequestHandler<GetAllUsersQuery, Result<PagedResult<UserDetailedDto>>>
{
    private readonly IUserRepository _users;
    private readonly IAuditLogRepository _audit;
    private readonly IUnitOfWork _uow;

    public GetAllUsersHandler(IUserRepository users, IAuditLogRepository audit, IUnitOfWork uow)
    {
        _users = users;
        _audit = audit;
        _uow = uow;
    }

    public async Task<Result<PagedResult<UserDetailedDto>>> Handle(GetAllUsersQuery req, CancellationToken ct)
    {
        var (users, total) = await _users.GetFilteredPagedAsync(
            req.Q,
            req.Role,
            req.Status,
            req.SpecializationId,
            req.UserTypeCodes,
            req.Page,
            req.PageSize,
            req.Sort,
            ct
        );

        var canViewPII = await _users.UserHasPermissionAsync(req.PerformedByUserId, "view_pii", ct);

        var dtos = users.Select(u => UserDetailedDto.FromEntity(u, canViewPII)).ToList();

        var result = new PagedResult<UserDetailedDto>(dtos, total, req.Page, req.PageSize);

        await _audit.AddAsync(AuditLog.Create(
            req.PerformedByUserId,
            "user_list",
            "User",
            null,
            null,
            $"User list retrieved with specialization filter = {req.SpecializationId}"
        ), ct);

        await _uow.SaveChangesAsync(ct);

        return Result<PagedResult<UserDetailedDto>>.Success(result);
    }
}