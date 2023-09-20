namespace DeckShufllerInterface;

using StrategyInterface;

public interface IDeckShufller
{
    void ShuffleDeck(Deck deck);
}

public class DeckShufller : IDeckShufller
{
    public void ShuffleDeck(Deck deck)
    {
        Card[] cards = deck.cards;
        int count = cards.Length;

        while (count > 1)
        {
            int i = Random.Shared.Next(count--);
            (cards[i], cards[count]) = (cards[count], cards[i]);
        }
    }
}