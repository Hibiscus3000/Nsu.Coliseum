using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.MassTransitOpponents;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Consumers;

public class CardPickedConsumer : IConsumer<CardNumberPicked>
{
    private const int MillisecondsDelay = 10;
    private const int TriesLimit = 50;

    private readonly ILogger<CardPickedConsumer> _logger;

    private readonly IRepo<Card[]> _deckRepo;
    private readonly IRepo<CardColor> _cardColorRepo;

    private readonly MassTransitResolver<QueueName> _queues;

    private readonly MassTransitOpponentType _opponentType;

    public CardPickedConsumer(ILogger<CardPickedConsumer> logger,
        IRepo<Card[]> deckRepo,
        IRepo<CardColor> cardColorRepo,
        MassTransitResolver<QueueName> queues,
        MassTransitOpponentType opponentType)
    {
        _logger = logger;

        _deckRepo = deckRepo;
        _cardColorRepo = cardColorRepo;
        _queues = queues;
        _opponentType = opponentType;
    }

    public async Task Consume(ConsumeContext<CardNumberPicked> context)
    {
        Guid id = context.CorrelationId!.Value;
        Card[]? deck = null;
        _logger.LogDebug($"Received CNP, GUID: {id}, card number: {context.Message.CardNumber}");
        int tries = 0;

        while (true)
        {
            deck = _deckRepo.GetT(id);
            if (++tries >= TriesLimit || null != deck) break;
            await Task.Delay(MillisecondsDelay);
        }

        bool success = null != deck;

        if (success)
        {
            CardColor cardColor = deck![context.Message.CardNumber].CardColor;
            _cardColorRepo.AddT(id, cardColor);
        }
        else
        {
            _logger.LogDebug($"Unable to find deck with given GUID: {id}");
        }

        ISendEndpoint sendEndpoint = await context.GetSendEndpoint(
            new Uri("exchange:" + _queues.GetName(QueueType.CardNumberAccepted).Value));
        await sendEndpoint.Send(new CardNumberAccepted
            {
                CorrelationId = id,
                Success = success,
                OpponentType = _opponentType
            },
            ctx => _logger.LogDebug($"Sent CNA, GUID: {ctx.CorrelationId!.Value}"));

        if (!success) throw new Exception($"{_opponentType}: Unable to fetch deck for given GUID = {id}");
    }
}