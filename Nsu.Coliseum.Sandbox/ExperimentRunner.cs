using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public class ExperimentRunner
{
    public bool Execute(IOpponents opponents, Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        int elonCardNum = opponents.GetCardNumber(OpponentType.Elon, elonDeck);
        int markCardNum = opponents.GetCardNumber(OpponentType.Mark, markDeck);

        return elonDeck[markCardNum].CardColor == markDeck[elonCardNum].CardColor;
    }
}