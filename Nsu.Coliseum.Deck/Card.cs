using System.Text.Json.Serialization;

namespace Nsu.Coliseum.Deck;

public class Card
{
    /// <summary>
    /// Used in <see cref="ToString()"/>
    /// </summary>
    private const char ClubSym = '\u2663';

    /// <summary>
    /// Used in <see cref="ToString()"/>
    /// </summary>
    private const char DiamondSym = '\u2662';

    /// <summary>
    /// Used in <see cref="ToString()"/>
    /// </summary>
    private const char HeartSym = '\u2661';

    /// <summary>
    /// Used in <see cref="ToString()"/>
    /// </summary>
    private const char SpadeSym = '\u2660';

    public CardColor CardColor { get; }
    public CardType CardType { get; }

    /// <summary>
    /// Number from 0 to 8 inclusive
    /// </summary>
    public int Number { get; }
    
    /// <param name="number">card number from 0 to 8 inclusive</param>
    /// <exception cref="ArgumentOutOfRangeException">If <c>number</c> is lesser than 0 or greater than 8</exception>
    /// <exception cref="ArgumentException">if <c>cardType</c> is not from <see cref="CardType"/>></exception>
    [JsonConstructor]
    public Card(CardType cardType, int number)
    {
        CardType = cardType;

        if (number < 0 || number > 8)
        {
            throw new ArgumentOutOfRangeException("Card number should be a number from 0 to 8 inclusive");
        }

        Number = number;

        // determine card color
        CardColor = cardType switch
        {
            CardType.Club => CardColor.Black,
            CardType.Diamond => CardColor.Red,
            CardType.Heart => CardColor.Red,
            CardType.Spade => CardColor.Black,
            _ => throw new ArgumentException("Unknown card type"),
        };
    }

    public Card(Card card) : this(card.CardType, card.Number)
    {
    }
    
    /// <param name="stringRepresentation">last symbol corresponds to card suit (<see cref="CardType"/>),
    /// all the other symbols corresponds to card nominal: 6, 7, 8, 9, 10, J, Q, K or A.</param>
    public static Card String2Card(string stringRepresentation)
    {
        CardType cardType = stringRepresentation[^1] switch
        {
            ClubSym => CardType.Club,
            DiamondSym => CardType.Diamond,
            HeartSym => CardType.Heart,
            SpadeSym => CardType.Spade
        };
        int number = stringRepresentation.Substring(0, stringRepresentation.Length - 1).Replace(" ", "") switch
        {
            "6" => 0,
            "7" => 1,
            "8" => 2,
            "9" => 3,
            "10" => 4,
            "J" => 5,
            "Q" => 6,
            "K" => 7,
            "A" => 8,
        };

        return new(cardType, number);
    }
    
    /// <returns>String card representation, which format is (nominal + suit). Example: " K♠".</returns>
    public override string ToString() => Number switch
    {
        0 => " 6",
        1 => " 7",
        2 => " 8",
        3 => " 9",
        4 => "10",
        5 => " J",
        6 => " Q",
        7 => " K",
        8 => " A",
    } + CardType switch
    {
        CardType.Club => ClubSym,
        CardType.Diamond => DiamondSym,
        CardType.Heart => HeartSym,
        CardType.Spade => SpadeSym,
    };

    protected bool Equals(Card other)
    {
        return CardType == other.CardType && Number == other.Number;
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((Card)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)CardType, Number);
    }
}

/// <summary>
/// Card suit
/// </summary>
public enum CardType
{
    Club,
    Diamond,
    Heart,
    Spade
}

public enum CardColor
{
    Red,
    Black
}