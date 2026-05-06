using Domain.Entities;
using Domain.Services;
using FluentAssertions;
using Xunit;

namespace Domain.Tests;

public class UserDomainServiceTests
{
    [Fact]
    public void AssignRole_Should_AddRole_When_NotAssigned()
    {
        var user = User.Create("a@a.com", "hash");
        var role = Role.Create("user");

        UserDomainService.AssignRole(user, role);

        user.UserRoles.Should().ContainSingle(ur => ur.RoleId == role.Id);
    }
}
