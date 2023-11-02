using MassTransit;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.MassTransit.Contracts
{
    public record PickCardFromDeck : CorrelatedBy<Guid>
    {
        public Card[] Deck { get; init; }
        public Guid CorrelationId { get; init; }
    }
}