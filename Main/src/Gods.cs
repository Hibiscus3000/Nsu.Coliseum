using Microsoft.Extensions.Hosting;
using StrategyInterface;

namespace Sandbox;

public class Gods : IHostedService
{
    private readonly ElonMusk _elonMusk;
    private readonly MarkZuckerberg _markZuckerberg;

    private readonly ExperimentRunner _experimentRunner;
    private readonly IDeckProvider _deckProvider;

    private readonly IHostApplicationLifetime _appLifetime;


    public Gods(ElonMusk elonMusk, MarkZuckerberg markZuckerberg, ExperimentRunner experimentRunner,
        IDeckProvider deckProvider, IHostApplicationLifetime applicationLifetime)
    {
        _elonMusk = elonMusk;
        _markZuckerberg = markZuckerberg;
        _experimentRunner = experimentRunner;
        _deckProvider = deckProvider;

        _appLifetime = applicationLifetime;
    }

    private void Play()
    {
        int numberOfSuccesses = 0;
        int numberOfExperiments = 0;

        Deck? deck;

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