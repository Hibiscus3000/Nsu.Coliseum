namespace StrategyInterface;

public class Deck
{
    private readonly int _numberOfCards;
    public readonly Card[] _cards;

    public Deck(int numberOfCards)
    {
        _numberOfCards = numberOfCards;
        int numberOfCardsInSuit = numberOfCards / 4;

        _cards = new Card[_numberOfCards];
        foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
        {
            for (int i = 0; i < numberOfCardsInSuit; ++i)
            {
                _cards[(int)cardType * numberOfCardsInSuit + i] = new Card(cardType, i % numberOfCardsInSuit);
            }
        }
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
 
            splited[groupIndex][cardIndex] = _cards[i];
        }

        return splited;
    }

    public void PrintDeck()
    {
        foreach (Card card in _cards)
        {
            Console.Write(card.ToString() + " ");
        }
        Console.WriteLine();
    }
}