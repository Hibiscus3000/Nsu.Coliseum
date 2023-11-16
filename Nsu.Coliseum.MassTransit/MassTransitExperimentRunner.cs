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

    public async Task Execute(long experimentNum, Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];
        
        Task elonDeckSendTask = SendDeck(experimentNum, elonDeck, OpponentType.Elon);
        Task markDeckSendTask = SendDeck(experimentNum, markDeck, OpponentType.Mark);
        await Task.WhenAll(elonDeckSendTask, markDeckSendTask);
    }

    private async Task SendDeck(long experimentNum, Card[] deck, OpponentType opponentType)
    {
        ISendEndpoint sendEndpoint = await _bus.GetSendEndpoint(
            new Uri("exchange:" + _queues.GetName(opponentType, QueueType.Deck).Value));
        await sendEndpoint.Send(new PickCardFromDeck { ExperimentNum = experimentNum, Deck = deck },
            context => _logger.LogDebug($"{experimentNum}: PCD sent, opponent type: {opponentType}"));
    }
}