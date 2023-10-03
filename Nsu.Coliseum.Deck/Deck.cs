namespace Nsu.Coliseum.Deck;

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

    public Deck(Deck other)
    {
        Card[] otherCards = other.Cards;
        Cards = new Card[otherCards.Length];
        for (int i = 0; i < Cards.Length; ++i)
        {
            Cards[i] = new Card(otherCards[i]);
        }
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

    protected bool Equals(Deck other)
    {
        if (Cards.Length != other.Cards.Length) return false;

        int numberOfCards = Cards.Length;

        for (int i = 0; i < numberOfCards; ++i)
        {
            if (!Cards[i].Equals(other.Cards[i])) return false;
        }

        return true;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Deck)obj);
    }

    public override int GetHashCode()
    {
        return Cards.GetHashCode();
    }

    public string ToString(string separator) => string.Join(separator, Cards.Select(c => c.ToString()).ToArray());
}