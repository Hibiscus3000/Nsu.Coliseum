namespace Nsu.Coliseum.Deck;

/// <summary>
/// Cards container
/// </summary>
public class Deck
{
    private readonly int _numberOfCards = 36;
    public Card[] Cards { get; }

    /// <summary>
    /// Default constructor which creates predefined deck, where card are order by suit in this order: Club, Diamond,
    /// Heart, Spade. And within one suit cards are ordered by nominal in ascending order. 
    /// </summary>
    /// <param name="numberOfCards">number of cards in deck</param>
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

    public Deck(Card[] cards) => Cards = cards;

    public Deck(Deck other)
    {
        Card[] otherCards = other.Cards;
        Cards = new Card[otherCards.Length];
        for (int i = 0; i < Cards.Length; ++i)
        {
            Cards[i] = new Card(otherCards[i]);
        }
    }

    /// <summary>
    /// Constructor used to transform deck db representation to object deck representation. 
    /// </summary>
    /// <param name="stringRepresentation">string deck representation</param>
    /// <param name="separator">string that is used to separate cards is <see cref="stringRepresentation"/></param>
    public Deck(string stringRepresentation, string separator = ";") =>
        Cards = Array.ConvertAll(stringRepresentation.Split(separator), Card.String2Card);

    /// <summary>
    /// Method is used to split deck into several arrays of card arrays, each top-level array goes to one opponent.
    /// </summary>
    /// <param name="numberOfGroups">number of upper level arrays.</param>
    /// <returns>Two-dimensional array, size of first axis is <see cref="numberOfGroups"/>,
    /// the second is <see cref="_numberOfCards"/>/<see cref="numberOfGroups"/></returns>
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

    /// <summary>
    /// Used to convert deck object representation to deck db representation
    /// </summary>
    /// <param name="separator">string that will be used to separate cards is &lt;see cref="stringRepresentation"/&gt;</param>
    /// <returns>String that is array of cards string representation (obtained with <c>ToString()</c> from <c>Card</c>)
    /// joined by <see cref="separator"/></returns>
    public string ToString(string separator) => string.Join(separator, Cards.Select(c => c.ToString()).ToArray());
}