using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.StrategyInterface;

namespace Nsu.Coliseum.MassTransit.Consumers;

public class PickCardFromDeckConsumer : IConsumer<PickCardFromDeck>
{
    private readonly ILogger<CardNumberPickedConsumer> _logger;
    private readonly MassTransitResolver<RoutingKey> _routingKeysResolver;

    private readonly Acknowledger _acknowledger;

    private readonly IStrategy _strategy;

    public PickCardFromDeckConsumer(ILogger<CardNumberPickedConsumer> logger,
        MassTransitResolver<RoutingKey> routingKeysResolver,
        Acknowledger acknowledger,
        IStrategy strategy)
    {
        _logger = logger;
        _routingKeysResolver = routingKeysResolver;

        _acknowledger = acknowledger;
        
        _strategy = strategy;
    }

    public async Task Consume(ConsumeContext<PickCardFromDeck> context)
    {
        Guid id = context.CorrelationId!.Value;

        int cardNumber = await _strategy.PickCardAsync(context.Message.Deck);
        
        await context.Publish(new CardNumberPicked
        {
            CorrelationId = id,
            CardNumber = cardNumber
        }, x =>
        {
            x.SetRoutingKey(_routingKeysResolver.GetName(QueueType.CardNumber).Value);
            _logger.LogDebug($"Sent CardNumberPicked Message, id: {id}, card number: {cardNumber}");
        });

        await _acknowledger.AddDeckAndSendAck(context);
    }
}