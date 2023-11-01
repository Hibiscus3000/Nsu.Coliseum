using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public class Gods : IHostedService
{
    private readonly IExperimentRunner _experimentRunner;
    private readonly IExperimentContext _experimentContext;

    private readonly IDeckProvider _deckProvider;

    private readonly IHostApplicationLifetime _appLifetime;

    private readonly ILogger<Gods> _logger;

    public Gods(IExperimentRunner experimentRunner,
        IExperimentContext experimentContext,
        IDeckProvider deckProvider,
        IHostApplicationLifetime applicationLifetime,
        ILogger<Gods> logger)
    {
        _experimentRunner = experimentRunner;
        _experimentContext = experimentContext;

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

        while (null != (deck = _deckProvider.GetDeck())) _experimentRunner.Execute(deck);

        _logger.LogDebug("Finished running experiments");

        _logger.LogDebug("Starting to wait experiment results");

        _logger.LogDebug("Finished waiting experiment results");

        _logger.LogInformation(_experimentContext.ToString());

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
}