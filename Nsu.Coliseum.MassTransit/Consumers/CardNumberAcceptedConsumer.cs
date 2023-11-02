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

    private readonly IRepo<CardColor> _temporaryCardColorStorage;

    public CardNumberAcceptedConsumer(ILogger<CardNumberAcceptedConsumer> logger,
        IExperimentContext experimentContext,
        IResolver<OpponentUrl> urlResolver,
        IRepo<CardColor> temporaryCardColorStorage)
    {
        _logger = logger;
        _experimentContext = experimentContext;

        _urlResolver = urlResolver;

        _temporaryCardColorStorage = temporaryCardColorStorage;
    }


    public async Task Consume(ConsumeContext<CardNumberAccepted> context)
    {
        OpponentType opponentType = context.Message.OpponentType.OpponentType;
        Guid id = context.CorrelationId!.Value;
        _logger.LogDebug($"Received CNA from {opponentType}, GUID: {id}");
        if (!context.Message.Success)
            throw new Exception($"Card number acceptation failed from {opponentType} (Main side).");
        CardColor cardColor = await SendCardColorHttpRequest(opponentType, id);
        _logger.LogDebug($"Received card color from {opponentType}: {cardColor}, GUID: {id}");
        AddCardColorToStorage(id, cardColor);
    }

    private const string GetCardColorUrlPath = "/api/Opponent/GetCardColor";

    private async Task<CardColor> SendCardColorHttpRequest(OpponentType opponentType, Guid id)
    {
        var requestUri = new Uri(_urlResolver.GetT(opponentType).Value + GetCardColorUrlPath + "?guid=" + id);
        return await _httpClient.GetFromJsonAsync<CardColor>(requestUri);
    }

    private void AddCardColorToStorage(Guid id, CardColor cardColor)
    {
        if (!_temporaryCardColorStorage.ContainsId(id))
        {
            _temporaryCardColorStorage.AddT(id, cardColor);
            _logger.LogDebug($"Added card color to temporary storage, GUID: {id}");
        }
        else
        {
            _experimentContext.AddExperimentResult(cardColor == _temporaryCardColorStorage.GetT(id));
        }
    }
}