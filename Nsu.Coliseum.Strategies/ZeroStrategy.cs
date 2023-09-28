using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;

namespace Nsu.Coliseum.Strategies;

public class ZeroStrategy : IStrategy
{
    public int PickCard(Card[] cards)
    {
        return 0;
    }
}