using Application.Security.Requirements;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Application.Security.Handlers;

public class SameUserHandler : AuthorizationHandler<SameUserRequirement, Guid>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        SameUserRequirement requirement,
        Guid resourceUserId)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        if (userIdClaim != null && Guid.TryParse(userIdClaim, out var currentUserId))
        {
            if (currentUserId == resourceUserId)
            {
                context.Succeed(requirement);
            }
        }

        return Task.CompletedTask;
    }
}
