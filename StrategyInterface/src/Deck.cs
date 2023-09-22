namespace StrategyInterface;

public class Deck
{
    private readonly int _numberOfCards = 36;
    public Card[] Cards { get; }

    public Deck(int numberOfCards)
    {
        _numberOfCards = numberOfCards;
        int numberOfCardsInSuit = numberOfCards / Enum.GetValues(typeof(CardType)).Length;

        Cards = new Card[_numberOfCards];
        foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
        {
            for (int i = 0; i < numberOfCardsInSuit; ++i)
            {
                Cards[(int)cardType * numberOfCardsInSuit + i] = new Card(cardType, i);
            }
        }
    }

    public Deck(Card[] cards)
    {
        Cards = cards;
    }

    public Deck(string stringRepresentation, string separator = ";")
    {
        Cards = Array.ConvertAll(stringRepresentation.Split(separator), Card.String2Card);
    }

    public Card[][] Split(int numberOfGroups)
    {
        var splited = new Card[numberOfGroups][];
        var numberOfCardsInGroup = _numberOfCards / numberOfGroups;
        for (int group = 0; group < numberOfGroups; ++group)
        {
            splited[group] = new Card[numberOfCardsInGroup];
        }

        for (int i = 0; i < _numberOfCards; ++i)
        {
            var groupIndex = i / numberOfCardsInGroup;
            var cardIndex = i % numberOfCardsInGroup;

            splited[groupIndex][cardIndex] = Cards[i];
        }

        return splited;
    }

    public string ToString(string separator) => string.Join(separator, Cards.Select(c => c.ToString()).ToArray());
}