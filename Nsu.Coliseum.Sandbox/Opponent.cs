using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;

namespace Nsu.Coliseum.Sandbox;

public enum OpponentType
{
    Elon,
    Mark
}

public interface IOpponents
{
    int GetCardNumber(OpponentType type, Card[] cards);
}

public class Opponents : IOpponents
{
    private readonly IStrategyResolver _strategyResolver;

    public Opponents(IStrategyResolver strategyResolver)
    {
        _strategyResolver = strategyResolver;
    }

    public int GetCardNumber(OpponentType type, Card[] cards)
    {
        return _strategyResolver.GetStrategy(type).PickCard(cards);
    }
}

public interface IStrategyResolver
{
    IStrategy GetStrategy(OpponentType type);
}

public class StrategyResolver : IStrategyResolver
{
    private readonly Dictionary<OpponentType, IStrategy> _strategies;

    public StrategyResolver(Dictionary<OpponentType, IStrategy> strategies)
    {
        _strategies = strategies;
    }

    public IStrategy GetStrategy(OpponentType type)
    {
        return _strategies[type];
    }
}