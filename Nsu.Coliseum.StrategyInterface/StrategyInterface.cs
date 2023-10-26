using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.StrategyInterface;

public interface IStrategy
{
    int PickCard(Card[] cards);

    async Task<int> PickCardAsync(Card[] cards)
    {
        return await Task.Run(() => PickCard(cards));
    }
}