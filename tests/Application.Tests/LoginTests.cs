using Application.Auth.Commands;
using Application.Common.Models;
using FluentAssertions;
using Domain.Abstractions;
using Moq;
using Xunit;
using System.Threading.Tasks;

namespace Application.Tests;

public class LoginTests
{
    [Fact]
    public async Task Login_Should_Fail_When_User_NotFound()
    {
        var users = new Mock<IUserRepository>();
        var refresh = new Mock<IRefreshTokenRepository>();
        var hasher = new Mock<IPasswordHasher>();
        var jwt = new Mock<IJwtTokenGenerator>();
        var clock = new Mock<IDateTimeProvider>();
        var uow = new Mock<IUnitOfWork>();
        var audit = new Mock<IAuditLogRepository>();

        var handler = new LoginUserHandler(hasher.Object, jwt.Object, clock.Object, users.Object, refresh.Object, uow.Object, audit.Object);
        var res = await handler.Handle(new LoginUserCommand("x@y.com", "pwd"), default);

        res.IsSuccess.Should().BeFalse();
        res.Error.Should().Be("Invalid credentials");
    }
}
