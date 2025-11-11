using Confluent.Kafka;
using DeliveryApp.Core.Application.Commands.CreateOrder;
using DeliveryApp.Infrastructure;
using MediatR;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Queues.Basket;

namespace DeliveryApp.Api.Adapters.Kafka.BasketConfirmed;

public class BasketConfirmedConsumer : BackgroundService
{
    private readonly IConsumer<Ignore, string> _consumer;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly string _topic;

    public BasketConfirmedConsumer(IServiceScopeFactory serviceScopeFactory, IOptions<Settings> settings)
    {
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        if (string.IsNullOrWhiteSpace(settings.Value.MessageBrokerHost))
        {
            throw new ArgumentException(nameof(settings.Value.MessageBrokerHost));
        }

        if (string.IsNullOrWhiteSpace(settings.Value.BasketConfirmedTopic))
        {
            throw new ArgumentException(nameof(settings.Value.BasketConfirmedTopic));
        }

        var consumerConfig = new ConsumerConfig
        {
            BootstrapServers = settings.Value.MessageBrokerHost,
            GroupId = "DeliveryConsumerGroup",
            EnableAutoOffsetStore = false,
            EnableAutoCommit = true,
            AutoOffsetReset = AutoOffsetReset.Earliest,
            EnablePartitionEof = true
        };

        _consumer = new ConsumerBuilder<Ignore, string>(consumerConfig).Build();
        _topic = settings.Value.BasketConfirmedTopic;
    }

    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<BasketConfirmedConsumer>>();
            _consumer.Subscribe(_topic);

            logger.LogInformation($"BasketConfirmedConsumer with topic {_topic} started");

            while (!cancellationToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromSeconds(3), cancellationToken);

                var consumeResult = _consumer.Consume(cancellationToken);
                if (consumeResult.IsPartitionEOF)
                {
                    continue;
                }

                logger.LogInformation($"Received message at {consumeResult.TopicPartitionOffset}\n Key:{consumeResult.Message.Key}\n Value:{consumeResult.Message.Value}");

                var basketConfirmedIntegrationEvent = JsonConvert.DeserializeObject<BasketConfirmedIntegrationEvent>(consumeResult.Message.Value);
                var createOrderCommandResult = CreateOrderCommand.Create(
                    Guid.Parse(basketConfirmedIntegrationEvent.BasketId),
                    basketConfirmedIntegrationEvent.Address.Street,
                    basketConfirmedIntegrationEvent.Volume);

                if (createOrderCommandResult.IsFailure)
                {
                    logger.LogInformation($"Error while creating order command: {createOrderCommandResult.Error}");
                }

                var response = await mediator.Send(createOrderCommandResult.Value, cancellationToken);
                if (response.IsFailure)
                {
                    logger.LogInformation(response.Error.Message);
                }

                try
                {
                    _consumer.StoreOffset(consumeResult);
                }
                catch (KafkaException e)
                {
                    logger.LogInformation($"Store Offset error: {e.Error.Reason}");
                }
            }
        }
        catch (OperationCanceledException)
        {
            _consumer.Close();
        }
    }
}