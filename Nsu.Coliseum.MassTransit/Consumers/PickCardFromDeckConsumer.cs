using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.MassTransitOpponents;
using Nsu.Coliseum.StrategyInterface;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Consumers;

public class PickCardFromDeckConsumer : IConsumer<PickCardFromDeck>
{
    private readonly ILogger<CardPickedConsumer> _logger;

    private readonly IRepo<Card[]> _deckRepo;
    private readonly MassTransitResolver<RoutingKey> _routingKeysResolver;

    private readonly IStrategy _strategy;

    public PickCardFromDeckConsumer(ILogger<CardPickedConsumer> logger,
        IRepo<Card[]> deckRepo,
        MassTransitResolver<RoutingKey> routingKeysResolver,
        IStrategy strategy)
    {
        _logger = logger;

        _deckRepo = deckRepo;
        _routingKeysResolver = routingKeysResolver;

        _strategy = strategy;
    }

    public async Task Consume(ConsumeContext<PickCardFromDeck> context)
    {
        Card[] cards = context.Message.Deck;
        Guid id = context.CorrelationId!.Value;

        _deckRepo.AddT(id, cards);
        _logger.LogDebug($"Received deck, GUID: {id}");
        await context.Publish(new CardNumberPicked
        {
            CorrelationId = id,
            CardNumber = await _strategy.PickCardAsync(cards)
        }, x => x.SetRoutingKey(_routingKeysResolver.GetName(QueueType.CardNumber).Value));
    }
}