using MassTransit;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.MassTransit.Contracts
{
    public record PickCardFromDeck
    {
        public Card[] Deck { get; init; }
        public long ExperimentNum { get; init; }
    }
}