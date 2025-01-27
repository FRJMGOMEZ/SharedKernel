using Microsoft.Extensions.Options;
using SharedKernel.Application.Events;
using SharedKernel.Application.Logging;
using SharedKernel.Domain.Events;
using SharedKernel.Infrastructure.Requests;
using SharedKernel.Infrastructure.Requests.Middlewares;

namespace SharedKernel.Infrastructure.RabbitMq.Events;

/// <summary>  </summary>
public class RabbitMqEventBus : RabbitMqPublisher, IEventBus
{
    private readonly IRequestSerializer _requestSerializer;
    private readonly IExecuteMiddlewaresService _executeMiddlewaresService;

    /// <summary>  </summary>
    public RabbitMqEventBus(
        ICustomLogger<RabbitMqPublisher> logger,
        IRequestSerializer requestSerializer,
        RabbitMqConnectionFactory config,
        IExecuteMiddlewaresService executeMiddlewaresService,
        IOptions<RabbitMqConfigParams> rabbitMqParams) : base(logger, config, rabbitMqParams)
    {
        _requestSerializer = requestSerializer;
        _executeMiddlewaresService = executeMiddlewaresService;
    }

    /// <summary>  </summary>
    public Task Publish(IEnumerable<DomainEvent> events, CancellationToken cancellationToken)
    {
        return Task.WhenAll(events.Select(@event => Publish(@event, cancellationToken)));
    }

    /// <summary>  </summary>
    public Task Publish(DomainEvent @event, CancellationToken cancellationToken)
    {
        return _executeMiddlewaresService.ExecuteAsync(@event, cancellationToken, (req, _) =>
        {
            var serializedDomainEvent = _requestSerializer.Serialize(req);

            return PublishTopic(serializedDomainEvent, req.GetEventName());
        });
    }
}
