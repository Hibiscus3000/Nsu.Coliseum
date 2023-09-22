using DeckShufflerInterface;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Sandbox;
using Strategy;
using StrategyInterface;

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
                // services.AddScoped<IDeckProvider, DBDeckProvider>();
                services.AddScoped<ElonMusk>(_ => new(new ZeroStrategy()));
                services.AddScoped<MarkZuckerberg>(_ => new(new ZeroStrategy()));
            });
    }
}