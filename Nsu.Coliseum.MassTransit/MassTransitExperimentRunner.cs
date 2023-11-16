using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.Sandbox;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit;

public class MassTransitExperimentRunner : IExperimentRunner
{
    private readonly ILogger<MassTransitExperimentRunner> _logger;

    private readonly IBus _bus;
    private readonly MassTransitResolver<QueueName> _queues;

    public MassTransitExperimentRunner(ILogger<MassTransitExperimentRunner> logger,
        IBus bus,
        MassTransitResolver<QueueName> queues)
    {
        _logger = logger;

        _bus = bus;
        _queues = queues;
    }

    public async Task Execute(Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        Guid id = Guid.NewGuid();
        Task elonDeckSendTask = SendDeck(id, elonDeck, OpponentType.Elon);
        Task markDeckSendTask = SendDeck(id, markDeck, OpponentType.Mark);
        await Task.WhenAll(elonDeckSendTask, markDeckSendTask);
    }

    private async Task SendDeck(Guid id, Card[] deck, OpponentType opponentType)
    {
        ISendEndpoint sendEndpoint = await _bus.GetSendEndpoint(
            new Uri("exchange:" + _queues.GetName(opponentType, QueueType.Deck).Value));
        await sendEndpoint.Send(new PickCardFromDeck { CorrelationId = id, Deck = deck },
            context => _logger.LogDebug($"Sent deck to {opponentType}, GUID: {context.CorrelationId!.Value}"));
    }
}