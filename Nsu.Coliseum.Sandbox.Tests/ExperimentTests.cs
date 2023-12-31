using Microsoft.Extensions.Logging;
using Moq;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.StrategyInterface;
using ReposAndResolvers;

namespace Nsu.Coliseum.Sandbox.Tests;

public class ExperimentTests
{
    private const int NumberOfCards = 36;

    private readonly IResolver<IStrategy> _strategyResolver;
    private IExperimentRunner _sut;
    private IExperimentContext _experimentContext;

    private readonly Mock<IDeckShuffler> _deckShufflerStub = new();

    public ExperimentTests()
    {
        Mock<IStrategy> elonStrategyStub = new();
        Mock<IStrategy> markStrategyStub = new();
        
        elonStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        markStrategyStub.Setup(m => m.PickCard(It.IsAny<Card[]>())).Returns(0);
        
        _strategyResolver = new Resolver<IStrategy>();
        _strategyResolver.AddT(OpponentType.Elon, elonStrategyStub.Object);
        _strategyResolver.AddT(OpponentType.Mark, markStrategyStub.Object);
        
        _experimentContext = new ExperimentContext(new Mock<ILogger<ExperimentContext>>().Object);
        _sut = new ExperimentRunner(new Opponents.Opponents(_strategyResolver), _experimentContext);
    }

    [Fact]
    public void Execute_ShuffleDeckCalledOneTime()
    {
        var deckShufflerMock = new Mock<IDeckShuffler>();
        deckShufflerMock.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()));

        _sut.Execute(0, new RandomDeckProvider(numberOfDecks: 1, numberOfCards: NumberOfCards,
            deckShuffler: deckShufflerMock.Object).GetDeck()!);

        deckShufflerMock.Verify(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()), Times.Once);
    }

    private void PredefinedDeckShuffle(Deck.Deck deck)
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
    public void Execute_LoosingDeckProvided_NumberOfVictoriesEqualsToZero()
    {
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()))
            .Callback((Deck.Deck deck) => PredefinedDeckShuffle(deck));

        _sut.Execute(0, new RandomDeckProvider(numberOfDecks: 1, numberOfCards: NumberOfCards,
            deckShuffler: _deckShufflerStub.Object).GetDeck()!);

        Assert.Equal(1, _experimentContext.GetNumberOfExperiments());
        Assert.Equal(0, _experimentContext.GetNumberOfVictories());
    }

    [Fact]
    public void Execute_WinningDeckProvided_NumberOfVictoriesEqualsToOne()
    {
        _deckShufflerStub.Setup(m => m.ShuffleDeck(It.IsAny<Deck.Deck>()))
            .Callback((Deck.Deck deck) =>
            {
                PredefinedDeckShuffle(deck);
                Card[] cards = deck.Cards;
                (cards[18], cards[19]) = (cards[19], cards[18]);
            });

        _sut.Execute(0, new RandomDeckProvider(numberOfDecks: 1, numberOfCards: NumberOfCards,
            deckShuffler: _deckShufflerStub.Object).GetDeck()!);

        Assert.Equal(1, _experimentContext.GetNumberOfExperiments());
        Assert.Equal(1, _experimentContext.GetNumberOfVictories());
    }
}