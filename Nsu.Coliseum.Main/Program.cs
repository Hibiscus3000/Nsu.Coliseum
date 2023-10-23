using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Sandbox;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;
using OpponentWepAPI;

namespace Nsu.Coliseum.Main;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args)
    {
        int numberOfCards =
            args.Length >= 2 && int.TryParse(args[1], out int result) ? result : 36;
        int numberOfExperiments =
            args.Length >= 1 && int.TryParse(args[0], out result) ? result : 1_000_000;

        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<Gods>();
                services.AddScoped<ExperimentRunner>();
                services.AddScoped<IDeckProvider, RandomDeckProvider>(_ => new RandomDeckProvider(
                    numberOfExperiments, numberOfCards, new DeckShuffler()));

                IStrategyResolver strategyResolver = new StrategyResolver(new Dictionary<OpponentType, IStrategy>
                {
                    [OpponentType.Elon] = new ZeroStrategy(),
                    [OpponentType.Mark] = new ZeroStrategy(),
                });

                // services.AddScoped<IDeckProvider, DbDeckProvider>(_ => new DbDeckProvider(50));

                // services.AddSingleton<IOpponents, Opponents>(_ => new Opponents(strategyResolver));

                services.AddSingleton<IOpponents, WebOpponents>(_ => new WebOpponents(strategyResolver));
            });
    }
}