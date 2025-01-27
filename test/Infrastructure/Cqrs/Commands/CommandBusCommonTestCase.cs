using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using SharedKernel.Application.Cqrs.Commands;
using SharedKernel.Application.Security;
using SharedKernel.Infrastructure;
using SharedKernel.Infrastructure.Cqrs.Commands;
using SharedKernel.Infrastructure.FluentValidation;
using SharedKernel.Infrastructure.FluentValidation.Requests.Middlewares;
using SharedKernel.Infrastructure.NetJson;
using SharedKernel.Infrastructure.Polly.Requests.Middlewares;
using SharedKernel.Integration.Tests.Events;
using SharedKernel.Testing.Infrastructure;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Integration.Tests.Cqrs.Commands;

public abstract class CommandBusCommonTestCase : InfrastructureTestCase<FakeStartup>
{
    protected override IServiceCollection ConfigureServices(IServiceCollection services)
    {
        return services
            .AddSharedKernel()
            .AddCommandsHandlers(typeof(SampleCommandWithResponseHandler))
            .AddFluentValidationValidators(typeof(SampleCommandValidator))
            .AddNetJsonSerializer()
            .AddValidationMiddleware()
            .AddRetryPolicyMiddleware<RetryPolicyExceptionHandler>(Configuration)
            .AddSingleton<SaveValueSingletonService>()
            .RemoveAll<IIdentityService>()
            .AddScoped<IIdentityService, HttpContextAccessorIdentityService>()
            .AddHttpContextAccessor();
    }

    protected async Task DispatchCommand()
    {
        var httpContextAccessor = GetRequiredService<IIdentityService>();

        if (httpContextAccessor != default)
        {
            httpContextAccessor.User =
                new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("Name", "Peter") }));

            httpContextAccessor.AddKeyValue("Authorization", "Prueba");
        }

        var commandBus = GetRequiredService<ICommandBus>();
        var command = new SampleCommand(3);
        await commandBus.Dispatch(command, CancellationToken.None);

        // Esperar a que terminen los manejadores de los eventos junto con la pol�tica de reintentos
        var saveValueSingletonService = GetRequiredService<SaveValueSingletonService>();
        await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);

        saveValueSingletonService.Id.Should().Be(command.Value);
    }

    protected async Task DispatchCommandAsync()
    {
        var httpContextAccessor = GetRequiredService<IIdentityService>();

        if (httpContextAccessor != default)
        {
            httpContextAccessor.User =
                new ClaimsPrincipal(new ClaimsIdentity(new List<Claim> { new Claim("Name", "Peter") }));

            httpContextAccessor.AddKeyValue("Authorization", "Prueba");
        }

        var commandBus = GetRequiredService<ICommandBusAsync>();
        var command = new SampleCommand(3);
        await commandBus.Dispatch(command, CancellationToken.None);

        // Esperar a que terminen los manejadores de los eventos junto con la pol�tica de reintentos
        var saveValueSingletonService = GetRequiredService<SaveValueSingletonService>();
        await Task.Delay(TimeSpan.FromSeconds(2), CancellationToken.None);

        saveValueSingletonService.Id.Should().Be(command.Value);
    }
}
