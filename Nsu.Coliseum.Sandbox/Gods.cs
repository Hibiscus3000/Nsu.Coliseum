using Microsoft.Extensions.Hosting;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public class Gods : IHostedService
{
    private readonly Opponent _elonMusk;
    private readonly Opponent _markZuckerberg;

    private readonly ExperimentRunner _experimentRunner;
    private readonly IDeckProvider _deckProvider;

    private readonly IHostApplicationLifetime _appLifetime;


    public Gods(IOpponentResolver opponentResolver, ExperimentRunner experimentRunner,
        IDeckProvider deckProvider, IHostApplicationLifetime applicationLifetime)
    {
        _elonMusk = opponentResolver.CreateOpponent(OpponentType.Elon);
        _markZuckerberg = opponentResolver.CreateOpponent(OpponentType.Mark);
        _experimentRunner = experimentRunner;
        _deckProvider = deckProvider;

        _appLifetime = applicationLifetime;
    }

    private void Play()
    {
        int numberOfSuccesses = 0;
        int numberOfExperiments = 0;

        Deck.Deck? deck;

        while (null != (deck = _deckProvider.GetDeck()))
        {
            if (_experimentRunner.Execute(_elonMusk, _markZuckerberg, deck))
            {
                ++numberOfSuccesses;
            }

            ++numberOfExperiments;
        }

        PrintResults(numberOfExperiments, numberOfSuccesses);

        _appLifetime.StopApplication();
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(Play, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }

    private static void PrintResults(int numberOfExperiments, int numberOfSuccesses)
    {
        Console.WriteLine("Number of experiments: " + numberOfExperiments);
        Console.WriteLine("Number of successes: " + numberOfSuccesses);
        Console.WriteLine("Statistics: "
                          + ((double)numberOfSuccesses * 100 / numberOfExperiments).ToString("N2") + "%");
    }
}