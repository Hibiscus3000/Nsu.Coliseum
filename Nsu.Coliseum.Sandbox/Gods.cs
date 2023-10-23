using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public class Gods : IHostedService
{
    private readonly IOpponents _opponents;

    private readonly ExperimentRunner _experimentRunner;
    private readonly IDeckProvider _deckProvider;

    private readonly IHostApplicationLifetime _appLifetime;

    private readonly ILogger<Gods> _logger;

    public Gods(IOpponents opponents,
        ExperimentRunner experimentRunner,
        IDeckProvider deckProvider,
        IHostApplicationLifetime applicationLifetime,
        ILogger<Gods> logger)
    {
        _opponents = opponents;

        _experimentRunner = experimentRunner;
        _deckProvider = deckProvider;

        _appLifetime = applicationLifetime;
        _logger = logger;
    }

    private void Play()
    {
        int numberOfSuccesses = 0;
        int numberOfExperiments = 0;

        Deck.Deck? deck;

        _logger.LogDebug("Starting experiments");

        while (null != (deck = _deckProvider.GetDeck()))
        {
            if (_experimentRunner.Execute(_opponents, deck))
            {
                ++numberOfSuccesses;
            }

            ++numberOfExperiments;
        }

        _logger.LogDebug("Finished experiments");

        LogResults(numberOfExperiments, numberOfSuccesses);

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

    private void LogResults(int numberOfExperiments, int numberOfSuccesses)
    {
        _logger.LogInformation("Number of experiments: " + numberOfExperiments +
                               ". Number of successes: " + numberOfSuccesses + ". Statistics: " +
                               ((double)numberOfSuccesses * 100 / numberOfExperiments)
                               .ToString("N2") + "%.");
    }
}