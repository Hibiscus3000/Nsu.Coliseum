using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Nsu.Coliseum.Deck;

namespace Nsu.Coliseum.Sandbox;

public class Gods : IHostedService
{
    private const int MillisecondsDelay = 100;
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

    private async Task Play()
    {
        Deck.Deck? deck;

        int numberOfExperiments = 0;

        _logger.LogDebug("Starting experiments");

        while (null != (deck = _deckProvider.GetDeck()))
        {
            _experimentRunner.Execute(deck);
            ++numberOfExperiments;
            _logger.LogDebug($"Ran {numberOfExperiments} experiments");
        }

        _logger.LogDebug("Finished running experiments");

        _logger.LogDebug("Starting to wait experiment results");

        int numberOfExperimentsFinished;
        while (numberOfExperiments != (numberOfExperimentsFinished = _experimentContext.GetNumberOfExperiments()))
        {
            _logger.LogDebug($"{numberOfExperimentsFinished}/{numberOfExperiments} experiments finished");
            await Task.Delay(MillisecondsDelay);
        }

        _logger.LogDebug("Finished waiting experiment results");

        _logger.LogInformation(_experimentContext.ToString());
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        Task.Run(async () =>
        {
            try
            {
                await Play();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message + Environment.NewLine + e.StackTrace);
            }
            finally
            {
                _appLifetime.StopApplication();
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}