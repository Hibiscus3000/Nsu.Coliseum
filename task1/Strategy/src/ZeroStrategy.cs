namespace Strategy;

using StrategyInterface;

public class ZeroStrategy : IStrategy
{
    public int PickCard(Card[] cards)
    {
        return 0;
    }
}
