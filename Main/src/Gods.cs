namespace Sandbox;

using Microsoft.Extensions.Hosting;
using DeckShufllerInterface;

public class Gods : IHostedService
{
    private readonly IGodsConfig _godsConfig;
    private readonly ElonMusk _elonMusk;
    private readonly MarkZuckerberg _markZuckerberg;

    private readonly Experiment _experiment;
    private readonly IDeckShufller _deckShuffler;

    private readonly IHostApplicationLifetime _appLifetime;


    public Gods(IGodsConfig godsConfig, ElonMusk elonMusk, MarkZuckerberg markZuckerberg, Experiment experiment,
        IDeckShufller deckShufller, IHostApplicationLifetime applicationLifetime)
    {
        _godsConfig = godsConfig;

        _elonMusk = elonMusk;
        _markZuckerberg = markZuckerberg;
        _experiment = experiment;
        _deckShuffler = deckShufller;

        _appLifetime = applicationLifetime;
    }

    public void Play()
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < _godsConfig.NumberOfExperiments; ++i)
        {
            if (_experiment.Execute(_elonMusk, _markZuckerberg, _deckShuffler, _godsConfig.NumberOfCardsInDeck))
            {
                ++numberOfSuccesses;
            }
        }

        PrintResults(_godsConfig.NumberOfExperiments, numberOfSuccesses);

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

    public interface IGodsConfig
    {
        int NumberOfExperiments {get;}
        int NumberOfCardsInDeck {get;}
    }

    public class DefaultGodsConfig : IGodsConfig
    {
        public int NumberOfExperiments {get;}

        public int NumberOfCardsInDeck {get;}

        public DefaultGodsConfig(int numberOfExperiments, int numberOfCardsInDeck)
        {
            NumberOfExperiments = numberOfExperiments;
            NumberOfCardsInDeck = numberOfCardsInDeck;
        }
    }
}