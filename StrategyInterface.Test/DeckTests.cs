namespace StrategyInterface.Test;

using StrategyInterface;

public class DeckTests
{
    [Fact]
    public void Deck_constructor_18Black18RedCards()
    {
        var numberOfCards = 36;
        var deck = new Deck(numberOfCards);
        Card[] cards = deck.cards;

        int numberOfRedCards = 0;
        int numberOfBlackCards = 0;
        foreach (Card card in cards)
        {
            if (CardColor.Red == card.CardColor)
            {
                ++numberOfRedCards;
            }
            if (CardColor.Black == card.CardColor)
            {
                ++numberOfBlackCards;
            }
        }

        Assert.Equal(18, numberOfRedCards);
        Assert.Equal(18, numberOfBlackCards);
    }
}