namespace Main.Test;

using DeckShufllerInterface;
using Moq;
using Sandbox;
using StrategyInterface;

public class ExperimentTests
{

    private readonly Mock<IStrategy> _elonStrategyStub = new();
    private readonly Mock<IStrategy> _markStrategyStub = new();
    private readonly ElonMusk _elon;
    private readonly MarkZuckerberg _mark;

    private readonly int _numberOfCards = 36;

    private readonly Experiment sut = new Experiment();

    private readonly Mock<IDeckShufller> _deckShufllerStub = new();

    public ExperimentTests()
    {
        _elonStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        _markStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        _elon = new(_elonStrategyStub.Object);
        _mark = new(_markStrategyStub.Object);

        sut = new Experiment();
    }

    [Fact]
    public void Experiment_Execute_ShuflleDeckCalledOneTime()
    {
        var deckShufllerMock = new Mock<IDeckShufller>();
        deckShufllerMock.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()));


        sut.Execute(_elon, _mark, deckShufllerMock.Object, _numberOfCards);


        deckShufllerMock.Verify(m => m.ShuffleDeck(It.IsAny<Deck>()), Times.Once);
    }

    private void PredifinedDeckShuflle(Deck deck)
    {
        Card[] cards = deck.cards;
        var numberOfSuits = Enum.GetValues(typeof(CardType)).Length;
        var numberOfCardsInSuit = _numberOfCards / Enum.GetValues(typeof(CardType)).Length;
        for (int i = 0; i < numberOfCardsInSuit; ++i)
        {
            foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
            {
                cards[i * numberOfSuits + (int)cardType] = new Card(cardType, i);
            }
        }
    }

    [Fact]
    public void Experiment_Execute_ReturnsFalse()
    {
        _deckShufllerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()))
            .Callback((Deck deck) => PredifinedDeckShuflle(deck));
        Assert.False(sut.Execute(_elon, _mark, _deckShufllerStub.Object, _numberOfCards));
    }

    [Fact]
    public void Experiment_Execute_ReturnsTrue()
    {
        _deckShufllerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()))
            .Callback((Deck deck) =>
            {
                PredifinedDeckShuflle(deck);
                Card[] cards = deck.cards;
                (cards[18], cards[19]) = (cards[19], cards[18]);
            });

        Assert.True(sut.Execute(_elon, _mark, _deckShufllerStub.Object, _numberOfCards));
    }
}