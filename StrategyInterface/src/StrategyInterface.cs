namespace StrategyInterface;

public interface IStrategy
{
    int PickCard(Card[] cards);
}

public class Card
{
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
            '\u2663' => CardType.Club,
            '\u2666' => CardType.Diamond,
            '\u2665' => CardType.Heart,
            '\u2660' => CardType.Spade
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
        CardType.Club => "\u2663",
        CardType.Diamond => "\u2666",
        CardType.Heart => "\u2665",
        CardType.Spade => "\u2660",
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