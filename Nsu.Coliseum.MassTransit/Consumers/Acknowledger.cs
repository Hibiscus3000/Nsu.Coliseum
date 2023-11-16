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
    
    private DeckAndCardNumRepository _deckAndCardNumRepository;
    
    private readonly IRepo<CardColor> _cardColorRepo;
    private MassTransitResolver<QueueName> _queues;

    private MassTransitOpponentType _opponentType;

    public Acknowledger(ILogger<Acknowledger> logger,
        DeckAndCardNumRepository deckAndCardNumRepository,
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
        Guid id = cardNumberPicked.CorrelationId;
        if (_deckAndCardNumRepository.AddCardNumber(id : id, cardNumber: cardNumberPicked.CardNumber))
            SendCardNumberAcceptedMessage(id, await GetSendEndpoint(context));;
    }

    public async Task AddDeckAndSendAck(ConsumeContext<PickCardFromDeck> context)
    {
        PickCardFromDeck pickCardFromDeck = context.Message;
        Guid id = context.CorrelationId!.Value;
        if (_deckAndCardNumRepository.AddDeck(id : id, deck: pickCardFromDeck.Deck))
            SendCardNumberAcceptedMessage(id, await GetSendEndpoint(context));
    }

    private async Task<ISendEndpoint> GetSendEndpoint(ConsumeContext context) =>
        await context.GetSendEndpoint(new Uri("exchange:" + _queues.GetName(QueueType.CardNumberAccepted).Value));
    
    private async Task SendCardNumberAcceptedMessage(Guid id, ISendEndpoint sendEndpoint)
    {
        Card[] deck = _deckAndCardNumRepository.GetDeck(id);
        int cardNumber = _deckAndCardNumRepository.GetCardNumber(id);
        CardColor cardColor = deck[cardNumber].CardColor;
        _cardColorRepo.AddT(id, cardColor);
        
        await sendEndpoint.Send(new CardNumberAccepted
            {
                CorrelationId = id,
                Success = true,
                OpponentType = _opponentType
            },
            ctx => _logger.LogDebug($"Sent CNA, GUID: {ctx.CorrelationId!.Value}"));
    }
}

public class DeckAndCardNumRepository
{
    private IDictionary<Guid, (Card[]? deck, int? cardNumber)> _dict = new Dictionary<Guid, (Card[]? deck, int? cardNumber)>();
    private IDictionary<Guid, Card[]?> _dict1 = new Dictionary<Guid,Card[]?>();
    
    private object _repoLock = new object();

    public bool AddCardNumber(Guid id, int cardNumber)
    {
        lock (_repoLock)
        {
            if (_dict.TryGetValue(id, out var t))
            {
                t.cardNumber = cardNumber;
                _dict[id] = t;
                return true;
            }
            _dict.Add(id, (null, cardNumber));
            return false;
        }
    }

    public bool AddDeck(Guid id, Card[] deck)
    {
        lock (_repoLock)
        {
            if (_dict.TryGetValue(id, out var t))
            {
                t.deck = deck;
                _dict[id] = t;
                return true;
            }
            _dict.Add(id, (deck, null));
            return false;
        }
    }

    public Card[] GetDeck(Guid id) => _dict[id].deck!;
    
    public int GetCardNumber(Guid id) => _dict[id].cardNumber!.Value;
} 