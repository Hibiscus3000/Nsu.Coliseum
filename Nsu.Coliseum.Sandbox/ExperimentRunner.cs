using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Opponents;
using ReposAndResolvers;

namespace Nsu.Coliseum.Sandbox;

public interface IExperimentRunner
{
    public Task Execute(long experimentNum, Deck.Deck deck);
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

    public abstract Task Execute(long experimentNum, Deck.Deck deck);
}

public class ExperimentRunner : AbstractExperimentRunner
{
    public ExperimentRunner(IOpponents opponents, IExperimentContext experimentContext) : base(opponents,
        experimentContext)
    {
    }

    public override Task Execute(long experimentNum, Deck.Deck deck)
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

    public override async Task Execute(long experimentNum, Deck.Deck deck)
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

    public long GetNumberOfExperiments();
    public long GetNumberOfVictories();
}

public class ExperimentContext : IExperimentContext
{
    private readonly ILogger<ExperimentContext> _logger;

    private long _numberOfExperiments = 0;
    private long _numberOfVictories = 0;

    public ExperimentContext(ILogger<ExperimentContext> logger) => _logger = logger;

    public void AddExperimentResult(bool victory)
    {
        Interlocked.Increment(ref _numberOfExperiments);
        if (victory) Interlocked.Increment(ref _numberOfVictories);
        _logger.LogDebug(
            $"Added experiment result in context, Total: {_numberOfExperiments}, Victories: {_numberOfVictories}");
    }

    public long GetNumberOfExperiments() => _numberOfExperiments;

    public long GetNumberOfVictories() => _numberOfVictories;

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