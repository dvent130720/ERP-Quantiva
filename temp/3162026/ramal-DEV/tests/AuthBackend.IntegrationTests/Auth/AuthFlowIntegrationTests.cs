using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using WireMock.Server;

namespace AuthBackend.IntegrationTests.Auth;

public class AuthFlowIntegrationTests : IAsyncLifetime
{
    private readonly IContainer _postgres = new ContainerBuilder()
        .WithImage("postgres:16")
        .WithEnvironment("POSTGRES_USER", "postgres")
        .WithEnvironment("POSTGRES_PASSWORD", "postgres")
        .WithEnvironment("POSTGRES_DB", "authdb")
        .WithPortBinding(55432, 5432)
        .Build();

    private readonly IContainer _redis = new ContainerBuilder()
        .WithImage("redis:7")
        .WithPortBinding(56379, 6379)
        .Build();

    private WireMockServer? _googleMock;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        await _redis.StartAsync();
        _googleMock = WireMockServer.Start(59001);
        // Aquí se pueden mockear endpoints de Google OAuth/JWKS según estrategia del proyecto.
    }

    public async Task DisposeAsync()
    {
        _googleMock?.Stop();
        await _postgres.StopAsync();
        await _redis.StopAsync();
    }

    [Fact]
    public void Should_Start_Dependencies_For_Auth_Integration()
    {
        Assert.True(_postgres.IsRunning);
        Assert.True(_redis.IsRunning);
        Assert.NotNull(_googleMock);
    }
}
