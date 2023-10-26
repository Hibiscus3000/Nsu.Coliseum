using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public interface IExperimentRunner
{
    public bool Execute(IOpponents opponents, Deck.Deck deck);
}

public class ExperimentRunner : IExperimentRunner
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

public class AsyncExperimentRunner : IExperimentRunner
{
    public bool Execute(IOpponents opponents, Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        Task<int> elonCardNum = opponents.GetCardNumberAsync(OpponentType.Elon, elonDeck);
        Task<int> markCardNum = opponents.GetCardNumberAsync(OpponentType.Mark, markDeck);

        return elonDeck[markCardNum.Result].CardColor == markDeck[elonCardNum.Result].CardColor;
    }
}