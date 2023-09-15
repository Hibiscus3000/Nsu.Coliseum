namespace StrategyInterface;

public class Deck
{
    private readonly int _numberOfCards;
    private Card[] cards { get; }

    public Deck(int numberOfCards)
    {
        _numberOfCards = numberOfCards;
        int numberOfCardsInSuit = numberOfCards / 4;

        cards = new Card[_numberOfCards];
        foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
        {
            for (int i = 0; i < numberOfCardsInSuit; ++i)
            {
                cards[(int)cardType * numberOfCardsInSuit + i] = new Card(cardType, i % numberOfCardsInSuit);
            }
        }

        ShuffleDeck(cards);
    }

    private static Card[] ShuffleDeck(Card[] cards)
    {
        int count = cards.Length;

        while (count > 1)
        {
            int i = Random.Shared.Next(count--);
            (cards[i], cards[count]) = (cards[count], cards[i]);
        }

        return cards;
    }

    public Card[][] Split(int numberOfGroups)
    {
        var splited = new Card[numberOfGroups][];
        int numberOfCardsInGroup = _numberOfCards / numberOfGroups;
        for (int group = 0; group < numberOfGroups; ++group) {
            splited[group] = new Card[numberOfCardsInGroup];
        }
 
        for (int i = 0; i < _numberOfCards; ++i) {
            var groupIndex = i / numberOfCardsInGroup;
            var cardIndex = i % numberOfCardsInGroup;
 
            splited[groupIndex][cardIndex] = cards[i];
        }

        return splited;
    }

    public void PrintDeck()
    {
        foreach (Card card in cards)
        {
            Console.Write(card.ToString() + " ");
        }
        Console.WriteLine();
    }
}