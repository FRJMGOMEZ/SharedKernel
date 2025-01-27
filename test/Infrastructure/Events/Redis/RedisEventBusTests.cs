using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Redis.Events;
using System.Threading.Tasks;
using Xunit;

namespace SharedKernel.Integration.Tests.Events.Redis;

[Collection("DockerHook")]
public class RedisEventBusTests : EventBusCommonTestCase
{
    protected override string GetJsonFile()
    {
        return "Events/Redis/appsettings.redis.json";
    }

    protected override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return base.ConfigureServices(services).AddRedisEventBus(Configuration);
    }

    [Fact]
    public async Task PublishDomainEventFromRedis()
    {
        await PublishDomainEvent();
    }
}
