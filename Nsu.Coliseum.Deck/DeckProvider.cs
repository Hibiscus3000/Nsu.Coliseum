namespace Nsu.Coliseum.Deck;

public interface IDeckProvider
{
    Deck? GetDeck();
}

public class RandomDeckProvider : IDeckProvider
{
    private readonly IDeckShuffler _deckShuffler;
    private readonly int _numberOfCards;

    private int _numberOfDecks;

    public RandomDeckProvider(DeckSizeAndNumOfDecks deckSizeAndNumOfDecks, IDeckShuffler? deckShuffler = null) :
        this(deckSizeAndNumOfDecks.NumberOfCardsInDeck, deckSizeAndNumOfDecks.NumberOfDecks, deckShuffler)
    {
    }

    public RandomDeckProvider(int numberOfCards, int numberOfDecks, IDeckShuffler? deckShuffler = null)
    {
        _numberOfCards = numberOfCards;
        _deckShuffler = deckShuffler ?? new DeckShuffler();

        _numberOfDecks = numberOfDecks;
    }

    public Deck? GetDeck()
    {
        if (_numberOfDecks-- <= 0)
        {
            return null;
        }

        var deck = new Deck(_numberOfCards);
        _deckShuffler.ShuffleDeck(deck);
        return deck;
    }
}