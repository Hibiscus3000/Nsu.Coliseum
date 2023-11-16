using MassTransit;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Contracts;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Consumers;

public class CardNumberPickedConsumer : IConsumer<CardNumberPicked>
{
    private readonly ILogger<CardNumberPickedConsumer> _logger;
    private readonly Acknowledger _acknowledger;

    public CardNumberPickedConsumer(ILogger<CardNumberPickedConsumer> logger,
        Acknowledger acknowledger)
    {
        _logger = logger;
        _acknowledger = acknowledger;
    }

    public async Task Consume(ConsumeContext<CardNumberPicked> context)
    {
        _logger.LogDebug($"{context.Message.ExperimentNum}: CNP, card number: {context.Message.CardNumber}");
        await _acknowledger.AddCardNumberAndSendAck(context);
    }
}