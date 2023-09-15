namespace StrategyInterface;

public interface IStrategy
{
    Card PickCard(Card[] cards);
}

public class Card
{
    public Card(CardType cardType, int number)
    {
        CardType = cardType;

        if (number < 0 || number > 8) {
            throw new ArgumentOutOfRangeException("card number should be a number from 0 to 8 inclusive");
        }
        Number = number;

        CardColor = cardType switch
        {
            CardType.Club => CardColor.Black,
            CardType.Diamond => CardColor.Red,
            CardType.Heart => CardColor.Red,
            CardType.Spade => CardColor.Black,
            _ => throw new ArgumentException("unknown card type"),
        };
    }

    public CardColor CardColor { get; }
    public CardType CardType { get; }
    public int Number { get; } // number from 0 to 8 inclusive

    public override string ToString()
    {
        return CardType switch
        {
            CardType.Club => "C",
            CardType.Diamond => "D",
            CardType.Heart => "H",
            CardType.Spade => "S",
        } + Number;
    }
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