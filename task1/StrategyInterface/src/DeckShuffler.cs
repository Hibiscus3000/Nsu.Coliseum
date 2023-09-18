namespace StrategyInterface;

public interface IDeckShuffler
{
    void ShuffleDeck(Deck deck);
}

public class DeckShuffler : IDeckShuffler
{
    public void ShuffleDeck(Deck deck)
    {
        Card[] cards = deck._cards;
        int count = cards.Length;

        while (count > 1)
        {
            int i = Random.Shared.Next(count--);
            (cards[i], cards[count]) = (cards[count], cards[i]);
        }
    }
}