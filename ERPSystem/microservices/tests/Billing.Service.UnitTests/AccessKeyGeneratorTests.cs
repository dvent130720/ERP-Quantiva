using Billing.Service.Application;

namespace Billing.Service.UnitTests;

public sealed class AccessKeyGeneratorTests
{
    [Fact]
    public void Generate_Should_Create_49_Length_Key()
    {
        var key = AccessKeyGenerator.Generate("1790012345001", new DateOnly(2026, 03, 01), "1");
        Assert.Equal(49, key.Length);
    }
}
