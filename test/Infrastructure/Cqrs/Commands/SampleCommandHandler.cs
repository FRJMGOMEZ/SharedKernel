﻿using SharedKernel.Application.Cqrs.Commands.Handlers;
using SharedKernel.Application.Security;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SharedKernel.Integration.Tests.Cqrs.Commands;

internal class SampleCommandHandler : ICommandRequestHandler<SampleCommand>
{
    private readonly IIdentityService _identityService;
    private readonly SaveValueSingletonService _saveValueSingletonService;

    public SampleCommandHandler(
        IIdentityService identityService,
        SaveValueSingletonService saveValueSingletonService)
    {
        _identityService = identityService;
        _saveValueSingletonService = saveValueSingletonService;
    }

    public Task Handle(SampleCommand command, CancellationToken cancellationToken)
    {
        if (_identityService.User.Claims.Any(e => e.Type == "Name" && e.Value == "Peter"))
            _saveValueSingletonService.SetId(command.Value);

        return Task.CompletedTask;
    }
}
