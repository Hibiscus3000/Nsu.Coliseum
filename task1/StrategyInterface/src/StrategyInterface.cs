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
        Number = number;

        switch (cardType)
        {
            case CardType.Club:
                CardColor = CardColor.Black;
                break;
            case CardType.Diamond:
                CardColor = CardColor.Red;
                break;
            case CardType.Heart:
                CardColor = CardColor.Red;
                break;
            case CardType.Spade:
                CardColor = CardColor.Black;
                break;
        }
    }

    public CardColor CardColor { get; }
    public CardType CardType { get; }
    public int Number { get; } // number from 0 to 8 inclusive

    public override string ToString()
    {
        string letter = "";
        switch (CardType)
        {
            case CardType.Club:
                letter = "C";
                break;
            case CardType.Diamond:
                letter = "D";
                break;
            case CardType.Heart:
                letter = "H";
                break;
            case CardType.Spade:
                letter = "S";
                break;
        }
        return letter + Number;
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