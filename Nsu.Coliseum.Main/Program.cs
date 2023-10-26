using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Sandbox;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;
using OpponentWebAPI;

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

                int elonUrlIndex = Array.IndexOf(args, "--elon-url") + 1;
                int markUrlIndex = Array.IndexOf(args, "--mark-url") + 1;

                if (0 == elonUrlIndex) throw new ArgumentException("Elon URL not specified");
                if (0 == markUrlIndex) throw new ArgumentException("Mark URL not specified");

                Dictionary<OpponentType, string> urls = new Dictionary<OpponentType, string>
                {
                    { OpponentType.Elon, args[elonUrlIndex] },
                    { OpponentType.Mark, args[markUrlIndex] }
                };

                services.AddSingleton<IOpponents, WebOpponents>(_ => new WebOpponents(strategyResolver,
                    urls));
            });
    }
}