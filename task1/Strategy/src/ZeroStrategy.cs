namespace Strategy;

using StrategyInterface;

public class ZeroStrategy : IStrategy
{
    public Card PickCard(Card[] cards)
    {
        return cards[0];
    }
}
