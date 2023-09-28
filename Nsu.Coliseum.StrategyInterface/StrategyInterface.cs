using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.StrategyInterface;

public interface IStrategy
{
    int PickCard(Card[] cards);
}