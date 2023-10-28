using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public interface IExperimentRunner
{
    public bool Execute(Deck.Deck deck);
}

public abstract class AbstractExperimentRunner : IExperimentRunner
{
    protected readonly IOpponents Opponents;

    public AbstractExperimentRunner(IOpponents opponents)
    {
        Opponents = opponents;
    }

    public abstract bool Execute(Deck.Deck deck);
}

public class ExperimentRunner : AbstractExperimentRunner
{
    public ExperimentRunner(IOpponents opponents) : base(opponents)
    {
    }

    public override bool Execute(Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        int elonCardNum = Opponents.GetCardNumber(OpponentType.Elon, elonDeck);

        int markCardNum = Opponents.GetCardNumber(OpponentType.Mark, markDeck);
        return elonDeck[markCardNum].CardColor == markDeck[elonCardNum].CardColor;
    }
}

public class AsyncExperimentRunner : AbstractExperimentRunner
{
    public AsyncExperimentRunner(IOpponents opponents) : base(opponents)
    {
    }

    public override bool Execute(Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        Task<int> elonCardNum = Opponents.GetCardNumberAsync(OpponentType.Elon, elonDeck);
        Task<int> markCardNum = Opponents.GetCardNumberAsync(OpponentType.Mark, markDeck);

        return elonDeck[markCardNum.Result].CardColor == markDeck[elonCardNum.Result].CardColor;
    }
}