using System.Net.Http.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.Sandbox;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Consumers;

public class CardNumberAcceptedConsumer : IConsumer<CardNumberAccepted>
{
    private readonly ILogger<CardNumberAcceptedConsumer> _logger;

    private readonly IExperimentContext _experimentContext;

    private readonly HttpClient _httpClient = new(new SocketsHttpHandler
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    });

    private readonly IResolver<OpponentUrl> _urlResolver;

    private readonly TupleRepository<CardColor, CardColor> _temporaryCardColorStorage;

    public CardNumberAcceptedConsumer(ILogger<CardNumberAcceptedConsumer> logger,
        IExperimentContext experimentContext,
        IResolver<OpponentUrl> urlResolver,
        TupleRepository<CardColor, CardColor> temporaryCardColorStorage)
    {
        _logger = logger;
        _experimentContext = experimentContext;

        _urlResolver = urlResolver;

        _temporaryCardColorStorage = temporaryCardColorStorage;
    }


    public async Task Consume(ConsumeContext<CardNumberAccepted> context)
    {
        OpponentType opponentType = context.Message.OpponentType.OpponentType;
        long experimentNum = context.Message.ExperimentNum;
        _logger.LogDebug($"{experimentNum}: CNA, opponent type: {opponentType}");
        if (!context.Message.Success)
            throw new CardAcceptationException(experimentNum, opponentType);
        CardColor cardColor = await SendCardColorHttpRequest(opponentType, experimentNum);
        _logger.LogDebug($"{experimentNum}: CC, opponent type: {opponentType}, card color: {cardColor}");
        AddCardColorToStorage(experimentNum, opponentType, cardColor);
    }

    private const string GetCardColorUrlPath = "/api/Opponent/GetCardColor";

    private async Task<CardColor> SendCardColorHttpRequest(OpponentType opponentType, long experimentNum)
    {
        var requestUri = new Uri(_urlResolver.GetT(opponentType).Value + GetCardColorUrlPath + "?experimentNum=" + experimentNum);
        return await _httpClient.GetFromJsonAsync<CardColor>(requestUri);
    }

    private void AddCardColorToStorage(long experimentNum, OpponentType opponentType, CardColor cardColor)
    {
        (bool ready ,CardColor? val) anotherColor = (0 == opponentType)
            ? _temporaryCardColorStorage.AddFirstOrGetSecond(experimentNum, cardColor) :
            _temporaryCardColorStorage.AddSecondOrGetFirst(experimentNum, cardColor);
        _logger.LogDebug($"{experimentNum}: added CC to temporary storage, opponent: {opponentType}, card color: {cardColor}");

        if (anotherColor.ready) _experimentContext.AddExperimentResult(anotherColor.val == cardColor);
    }
}

public class CardAcceptationException : Exception
{
    public CardAcceptationException(long experimentNum, OpponentType opponentType) : base($"Failed to accept card from {opponentType}, EXP NUM: {experimentNum}")
    {
        
    }

    public CardAcceptationException()
    {
    }

    public CardAcceptationException(string? message) : base(message)
    {
    }

    public CardAcceptationException(string? message, Exception? innerException) : base(message, innerException)
    {
    }
}