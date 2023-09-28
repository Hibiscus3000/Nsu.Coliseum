namespace Nsu.Coliseum.Deck;

public class Card
{
    private const char ClubSym = '\u2663';
    private const char DiamondSym = '\u2662';
    private const char HeartSym = '\u2661';
    private const char SpadeSym = '\u2660';


    public CardColor CardColor { get; }
    private readonly CardType _cardType;
    private readonly int _number; // number from 0 to 8 inclusive

    public Card(CardType cardType, int number)
    {
        _cardType = cardType;

        if (number < 0 || number > 8)
        {
            throw new ArgumentOutOfRangeException("card number should be a number from 0 to 8 inclusive");
        }

        _number = number;

        CardColor = cardType switch
        {
            CardType.Club => CardColor.Black,
            CardType.Diamond => CardColor.Red,
            CardType.Heart => CardColor.Red,
            CardType.Spade => CardColor.Black,
            _ => throw new ArgumentException("unknown card type"),
        };
    }

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

    public override string ToString() => _number switch
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
    } + _cardType switch
    {
        CardType.Club => ClubSym,
        CardType.Diamond => DiamondSym,
        CardType.Heart => HeartSym,
        CardType.Spade => SpadeSym,
    };
}

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