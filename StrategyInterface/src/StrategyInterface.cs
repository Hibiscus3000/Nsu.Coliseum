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


    public override string ToString() => _cardType switch
    {
        CardType.Club => "C",
        CardType.Diamond => "D",
        CardType.Heart => "H",
        CardType.Spade => "S",
    } + _number;
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