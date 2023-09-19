namespace Sandbox;

using StrategyInterface;

public abstract class Opponent
{

    private readonly IStrategy _strategy;

    public Opponent(IStrategy strategy)
    {
        _strategy = strategy;
    }

    public int UseStrategy(Card[] cards) => _strategy.PickCard(cards);
}

public class ElonMusk : Opponent
{
    public ElonMusk(IStrategy strategy) : base(strategy)
    {
    }
}

public class MarkZuckerberg : Opponent
{
    public MarkZuckerberg(IStrategy strategy) : base(strategy)
    {
    }
}