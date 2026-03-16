using AuthBackend.Application.Contracts;
using AuthBackend.Application.Models;
using AuthBackend.Application.Services;
using AuthBackend.Domain.Entities;
using Moq;

namespace AuthBackend.UnitTests.Services;

public class AuthServiceTests
{
    [Fact]
    public async Task Register_ShouldHashPassword_AndReturnTokens()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), default)).ReturnsAsync((User?)null);
        users.Setup(x => x.AddAsync(It.IsAny<User>(), default)).Returns(Task.CompletedTask);

        var refresh = new Mock<IRefreshTokenRepository>();
        refresh.Setup(x => x.SaveAsync(It.IsAny<RefreshToken>(), default)).Returns(Task.CompletedTask);

        var hasher = new Mock<IPasswordHasher>();
        hasher.Setup(x => x.Hash("Secure123!")).Returns("hashed");
        var jwt = new Mock<IJwtTokenService>();
        jwt.Setup(x => x.GenerateAccessToken(It.IsAny<User>())).Returns("access");
        jwt.Setup(x => x.GenerateRefreshToken()).Returns("refresh");

        var google = new Mock<IGoogleTokenValidator>();
        var state = new Mock<IStateStore>();

        var sut = new AuthService(users.Object, refresh.Object, hasher.Object, jwt.Object, google.Object, state.Object);

        var result = await sut.RegisterAsync(new RegisterRequest("test@mail.com", "Secure123!"));

        Assert.Equal("access", result.AccessToken);
        Assert.Equal("refresh", result.RefreshToken);
        hasher.Verify(x => x.Hash("Secure123!"), Times.Once);
    }
}
