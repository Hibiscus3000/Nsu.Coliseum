namespace Nsu.Coliseum.Deck.Tests;

public class DeckTests
{
    [Fact]
    public void Constructor_18Black18RedCards()
    {
        var numberOfCards = 36;
        var deck = new Deck(numberOfCards);
        Card[] cards = deck.Cards;

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