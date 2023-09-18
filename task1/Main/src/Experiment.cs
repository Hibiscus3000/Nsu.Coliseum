using DeckShufllerInterface;
using StrategyInterface;

public class Experiment
{
    public bool Execute(Opponent elon, Opponent mark,
        IDeckShufller deckShuffler, int numberOfCards)
    {
        var deck = new Deck(numberOfCards);

        deckShuffler.ShuffleDeck(deck);

        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        int elonCardNum = elon.UseStrategy(elonDeck);
        int markCardNum = mark.UseStrategy(markDeck);

        return elonDeck[markCardNum].CardColor == markDeck[elonCardNum].CardColor;
    }
}