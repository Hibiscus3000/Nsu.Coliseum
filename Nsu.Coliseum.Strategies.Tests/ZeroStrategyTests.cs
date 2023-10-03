namespace Nsu.Coliseum.Strategies.Tests;

public class ZeroStrategyTests
{
    [Fact]
    public void PickCard_AnyDeck_ReturnsZero()
    {
        var zeroStrategy = new ZeroStrategy();
        var deck = new Deck.Deck(36);

        var result = zeroStrategy.PickCard(deck.Split(2)[0]);

        Assert.Equal(0, result);
    }
}