﻿using Microsoft.Extensions.DependencyInjection;
using SharedKernel.Infrastructure.Events;
using System.Threading.Tasks;
using Xunit;

namespace SharedKernel.Integration.Tests.Events.InMemory;

public class InMemoryEventBusTests : EventBusCommonTestCase
{
    protected override string GetJsonFile()
    {
        return "Events/InMemory/appsettings.inMemory.json";
    }

    protected override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return base.ConfigureServices(services).AddInMemoryEventBus(Configuration);
    }

    [Fact]
    public async Task PublishDomainEventFromMemory()
    {
        await PublishDomainEvent();
    }
}
