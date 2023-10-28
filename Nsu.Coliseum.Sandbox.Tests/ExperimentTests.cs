using Moq;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;

namespace Nsu.Coliseum.Sandbox.Tests;

public class ExperimentTests
{
    private const int NumberOfCards = 36;

    private readonly ExperimentRunner _sut;

    private readonly Mock<IDeckShuffler> _deckShufflerStub = new();

    public ExperimentTests()
    {
        Mock<IStrategy> elonStrategyStub = new();
        Mock<IStrategy> markStrategyStub = new();
        elonStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        markStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        var strategyResolver = new StrategyResolver(new Dictionary<OpponentType, IStrategy>
        {
            [OpponentType.Elon] = elonStrategyStub.Object,
            [OpponentType.Mark] = markStrategyStub.Object
        });

        _sut = new ExperimentRunner(new Opponents(strategyResolver));
    }

    [Fact]
    public void Execute_ShuffleDeckCalledOneTime()
    {
        var deckShufflerMock = new Mock<IDeckShuffler>();
        deckShufflerMock.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()));


        _sut.Execute(new RandomDeckProvider(1, NumberOfCards, deckShufflerMock.Object).GetDeck());

        deckShufflerMock.Verify(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()), Times.Once);
    }

    private void PredifinedDeckShuflle(Deck.Deck deck)
    {
        Card[] cards = deck.Cards;
        var numberOfSuits = Enum.GetValues(typeof(CardType)).Length;
        var numberOfCardsInSuit = NumberOfCards / Enum.GetValues(typeof(CardType)).Length;
        for (int i = 0; i < numberOfCardsInSuit; ++i)
        {
            foreach (CardType cardType in Enum.GetValues(typeof(CardType)))
            {
                cards[i * numberOfSuits + (int)cardType] = new Card(cardType, i);
            }
        }
    }

    [Fact]
    public void Execute_LoosingDeckProvided_ReturnsFalse()
    {
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()))
            .Callback((Deck.Deck deck) => PredifinedDeckShuflle(deck));
        Assert.False(_sut.Execute(new RandomDeckProvider(1, NumberOfCards, _deckShufflerStub.Object).GetDeck()));
    }

    [Fact]
    public void Execute_WinningDeckProvided_ReturnsTrue()
    {
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()))
            .Callback((Deck.Deck deck) =>
            {
                PredifinedDeckShuflle(deck);
                Card[] cards = deck.Cards;
                (cards[18], cards[19]) = (cards[19], cards[18]);
            });

        Assert.True(_sut.Execute(new RandomDeckProvider(1, NumberOfCards, _deckShufflerStub.Object).GetDeck()));
    }
}