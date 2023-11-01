using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Opponents;
using ReposAndResolvers;

namespace Nsu.Coliseum.Sandbox;

public interface IExperimentRunner
{
    public Task Execute(Deck.Deck deck);
}

public abstract class AbstractExperimentRunner : IExperimentRunner
{
    protected readonly IOpponents Opponents;
    protected readonly IExperimentContext ExperimentContext;

    public AbstractExperimentRunner(IOpponents opponents, IExperimentContext experimentContext)
    {
        Opponents = opponents;
        ExperimentContext = experimentContext;
    }

    public abstract Task Execute(Deck.Deck deck);
}

public class ExperimentRunner : AbstractExperimentRunner
{
    public ExperimentRunner(IOpponents opponents, IExperimentContext experimentContext) : base(opponents,
        experimentContext)
    {
    }

    public override Task Execute(Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        int markCardNum = Opponents.GetCardNumber(OpponentType.Elon, elonDeck);

        int elonCardNum = Opponents.GetCardNumber(OpponentType.Mark, markDeck);

        ExperimentContext.AddExperimentResult(markDeck[markCardNum].CardColor == elonDeck[elonCardNum].CardColor);

        return Task.CompletedTask;
    }
}

public class ExperimentRunnerAsync : AbstractExperimentRunner
{
    public ExperimentRunnerAsync(IOpponents opponents, IExperimentContext experimentContext) : base(opponents,
        experimentContext)
    {
    }

    public override async Task Execute(Deck.Deck deck)
    {
        Card[][] splitedDeck = deck.Split(2);

        Card[] elonDeck = splitedDeck[0];
        Card[] markDeck = splitedDeck[1];

        Task<int> elonCardNum = Opponents.GetCardNumberAsync(OpponentType.Elon, elonDeck);
        Task<int> markCardNum = Opponents.GetCardNumberAsync(OpponentType.Mark, markDeck);

        ExperimentContext.AddExperimentResult(markDeck[await markCardNum].CardColor ==
                                              elonDeck[await elonCardNum].CardColor);
    }
}

public interface IExperimentContext
{
    public void AddExperimentResult(bool victory);

    public int GetNumberOfExperiments();
    public int GetNumberOfVictories();
}

public class ExperimentContext : IExperimentContext
{
    private int _numberOfExperiments = 0;
    private int _numberOfVictories = 0;

    public void AddExperimentResult(bool victory)
    {
        Interlocked.Increment(ref _numberOfExperiments);
        if (victory) Interlocked.Increment(ref _numberOfVictories);
    }

    public int GetNumberOfExperiments() => _numberOfExperiments;

    public int GetNumberOfVictories() => _numberOfVictories;

    public override string ToString()
    {
        string statistics = "";
        if (0 != _numberOfExperiments)
        {
            statistics = " Statistics: " +
                         ((double)_numberOfVictories * 100 / _numberOfExperiments)
                         .ToString("N2") + "%.";
        }

        return "Number of experiments: " + _numberOfExperiments +
               ". Number of successes: " + _numberOfVictories + "." + statistics;
    }
}