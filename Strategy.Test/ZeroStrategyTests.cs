namespace Strategy.Test;

using Strategy;
using StrategyInterface;

public class ZeroStrategyTests
{
    [Fact]
    public void ZeroStrategy_PickCard_ReturnsZero()
    {
        var zeroStrategy = new ZeroStrategy();
        var deck = new Deck(36);

        var result = zeroStrategy.PickCard(deck.Split(2)[0]);

        Assert.Equal(0, result);
    }
}