using StrategyInterface;

public class Opponent
{

    private readonly IStrategy _strategy;

    public Opponent(IStrategy strategy)
    {
        _strategy = strategy;
    }

    public int UseStrategy(Card[] cards) => _strategy.PickCard(cards);
}