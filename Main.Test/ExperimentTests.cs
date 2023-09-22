using DeckShufflerInterface;
using Moq;
using Sandbox;
using StrategyInterface;

namespace Main.Test;

public class ExperimentTests
{
    private readonly Mock<IStrategy> _elonStrategyStub = new();
    private readonly Mock<IStrategy> _markStrategyStub = new();
    private readonly ElonMusk _elon;
    private readonly MarkZuckerberg _mark;

    private readonly int _numberOfCards = 36;

    private readonly ExperimentRunner _sut = new ExperimentRunner();

    private readonly Mock<IDeckShuffler> _deckShufflerStub = new();

    public ExperimentTests()
    {
        _elonStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        _markStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        _elon = new(_elonStrategyStub.Object);
        _mark = new(_markStrategyStub.Object);
    }

    [Fact]
    public void Experiment_Execute_ShuffleDeckCalledOneTime()
    {
        var deckShufflerMock = new Mock<IDeckShuffler>();
        deckShufflerMock.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()));


        _sut.Execute(_elon, _mark,
            new RandomDeckProvider(1, _numberOfCards, deckShufflerMock.Object).GetDeck());

        deckShufflerMock.Verify(m => m.ShuffleDeck(It.IsAny<Deck>()), Times.Once);
    }

    private void PredifinedDeckShuflle(Deck deck)
    {
        Card[] cards = deck.Cards;
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
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()))
            .Callback((Deck deck) => PredifinedDeckShuflle(deck));
        Assert.False(_sut.Execute(_elon, _mark,
            new RandomDeckProvider(1, _numberOfCards, _deckShufflerStub.Object).GetDeck()));
    }

    [Fact]
    public void Experiment_Execute_ReturnsTrue()
    {
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck>()))
            .Callback((Deck deck) =>
            {
                PredifinedDeckShuflle(deck);
                Card[] cards = deck.Cards;
                (cards[18], cards[19]) = (cards[19], cards[18]);
            });

        Assert.True(_sut.Execute(_elon, _mark,
            new RandomDeckProvider(1, _numberOfCards, _deckShufflerStub.Object).GetDeck()));
    }
}