using System.Collections.Concurrent;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Consumers;

public sealed class Acknowledger
{
    private ILogger<Acknowledger> _logger;
    
    private TupleRepository<Card[], int> _deckAndCardNumRepository;
    
    private readonly IRepo<CardColor> _cardColorRepo;
    private MassTransitResolver<QueueName> _queues;

    private MassTransitOpponentType _opponentType;

    public Acknowledger(ILogger<Acknowledger> logger,
        TupleRepository<Card[], int> deckAndCardNumRepository,
        IRepo<CardColor> cardColorRepo,
        MassTransitResolver<QueueName> queues,
        MassTransitOpponentType opponentType)
    {
        _deckAndCardNumRepository = deckAndCardNumRepository;
        _cardColorRepo = cardColorRepo;
        
        _queues = queues;

        _opponentType = opponentType;
        _logger = logger;
    } 

    public async Task AddCardNumberAndSendAck(ConsumeContext<CardNumberPicked> context)
    {
        CardNumberPicked cardNumberPicked = context.Message;
        long experimentNum = context.Message.ExperimentNum;
        if (_deckAndCardNumRepository.AddSecond(experimentNum : experimentNum, second: cardNumberPicked.CardNumber))
            SendCardNumberAcceptedMessage(experimentNum, await GetSendEndpoint(context));;
    }

    public async Task AddDeckAndSendAck(ConsumeContext<PickCardFromDeck> context)
    {
        PickCardFromDeck pickCardFromDeck = context.Message;
        long experimentNum = context.Message.ExperimentNum; 
        if (_deckAndCardNumRepository.AddFirst(experimentNum : experimentNum, first: pickCardFromDeck.Deck))
            SendCardNumberAcceptedMessage(experimentNum, await GetSendEndpoint(context));
    }

    private async Task<ISendEndpoint> GetSendEndpoint(ConsumeContext context) =>
        await context.GetSendEndpoint(new Uri("exchange:" + _queues.GetName(QueueType.CardNumberAccepted).Value));
    
    private async Task SendCardNumberAcceptedMessage(long experimentNum, ISendEndpoint sendEndpoint)
    {
        Card[] deck = _deckAndCardNumRepository.GetFirst(experimentNum);
        int cardNumber = _deckAndCardNumRepository.GetSecond(experimentNum);
        CardColor cardColor = deck[cardNumber].CardColor;
        _cardColorRepo.AddT(experimentNum, cardColor);
        
        await sendEndpoint.Send(new CardNumberAccepted
            {
                ExperimentNum = experimentNum,
                Success = true,
                OpponentType = _opponentType
            },
            ctx => _logger.LogDebug($" {experimentNum}: CNA Sent"));
    }
}