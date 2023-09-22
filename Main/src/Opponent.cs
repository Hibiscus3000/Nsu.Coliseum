using StrategyInterface;

namespace Sandbox;

public enum OpponentType
{
    Elon,
    Mark
}

public interface IOpponentResolver
{
    Opponent CreateOpponent(OpponentType type);
}

public class OpponentResolver : IOpponentResolver
{
    private readonly Dictionary<OpponentType, IStrategy> _strategies;

    public OpponentResolver(Dictionary<OpponentType, IStrategy> strategies)
    {
        _strategies = strategies;
    }

    public Opponent CreateOpponent(OpponentType type) => type switch
    {
        OpponentType.Elon => new ElonMusk(_strategies[OpponentType.Elon]),
        OpponentType.Mark => new MarkZuckerberg(_strategies[OpponentType.Mark])
    };
}

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