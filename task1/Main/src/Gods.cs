using Microsoft.Extensions.Hosting;
using DeckShufllerInterface;

public class Gods : IHostedService
{
    private readonly int _numberOfCards = 36;
    private readonly int _numberOfExperiments = 1_000_000;
    private readonly ElonMusk _elonMusk;
    private readonly MarkZuckerberg _markZuckerberg;

    private readonly Experiment _experiment;
    private readonly IDeckShufller _deckShuffler;

    private readonly IHostApplicationLifetime _appLifetime;

    public Gods(ElonMusk elonMusk, MarkZuckerberg markZuckerberg, Experiment experiment,
        IDeckShufller deckShufller, IHostApplicationLifetime applicationLifetime)
    {
        _elonMusk = elonMusk;
        _markZuckerberg = markZuckerberg;
        _experiment = experiment;
        _deckShuffler = deckShufller;

        _appLifetime = applicationLifetime;
    }

    public void Play()
    {
        int numberOfSuccesses = 0;

        for (int i = 0; i < _numberOfExperiments; ++i)
        {
            if (_experiment.Execute(_elonMusk, _markZuckerberg, _deckShuffler, _numberOfCards))
            {
                ++numberOfSuccesses;
            }
        }

        PrintResults(_numberOfExperiments, numberOfSuccesses);

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