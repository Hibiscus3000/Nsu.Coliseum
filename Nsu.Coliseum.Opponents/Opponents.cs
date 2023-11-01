using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;
using ReposAndResolvers;

namespace Nsu.Coliseum.Opponents;

public interface IOpponents
{
    int GetCardNumber(OpponentType type, Card[] cards);

    Task<int> GetCardNumberAsync(OpponentType type, Card[] cards);

    static OpponentType GetOpposite(OpponentType type) => type switch
    {
        OpponentType.Elon => OpponentType.Mark,
        OpponentType.Mark => OpponentType.Elon
    };
}

public class Opponents : IOpponents
{
    private readonly IResolver<IStrategy> _strategyResolver;

    public Opponents(IResolver<IStrategy> strategyResolver) => _strategyResolver = strategyResolver;

    public int GetCardNumber(OpponentType type, Card[] cards) =>
        _strategyResolver.GetT(type).PickCard(cards);

    public async Task<int> GetCardNumberAsync(OpponentType type, Card[] cards) =>
        await _strategyResolver.GetT(type).PickCardAsync(cards);
}