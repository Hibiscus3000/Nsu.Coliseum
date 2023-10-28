using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.Coliseum.Database;
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
        return Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                IConfiguration config = hostContext.Configuration;

                services.AddHostedService<Gods>();
                ConfigureExperimentRunner(config, services);
                ConfigureDeckProvider(config, services);
                ConfigureOpponents(config, services);
            });
    }

    private static void ConfigureDeckProvider(IConfiguration config,
        IServiceCollection services) =>
        services.AddScoped<IDeckProvider>(_ => config["DeckProvider"] switch
        {
            "random" => new RandomDeckProvider(
                numberOfDecks: null != config["NumberOfExperiments"] &&
                               int.TryParse(config["NumberOfExperiments"], out int resultNE)
                    ? resultNE
                    : 36,
                numberOfCards: null != config["NumberOfCards"] &&
                               int.TryParse(config["NumberOfCards"], out int resultNC)
                    ? resultNC
                    : 36),
            "database" => new DbDeckProvider()
        });

    private static void ConfigureExperimentRunner(IConfiguration config,
        IServiceCollection services)
    {
        if (Convert.ToBoolean(config["Async"])) services.AddScoped<IExperimentRunner, ExperimentRunner>();
        else services.AddScoped<IExperimentRunner, AsyncExperimentRunner>();
    }

    private static void ConfigureOpponents(IConfiguration config,
        IServiceCollection services)
    {
        IStrategyResolver strategyResolver = CreateStrategyResolver(config);
        services.AddScoped<IOpponents>(_ => config["Opponents:Type"] switch
        {
            "web" => new WebOpponents(strategyResolver, CreateUrlDict(config)),
            "default" => new Opponents(strategyResolver)
        });
    }

    private static Dictionary<OpponentType, string> CreateUrlDict(IConfiguration config) =>
        new()
        {
            { OpponentType.Elon, config["Opponents:ElonUrl"] },
            { OpponentType.Mark, config["Opponents:MarkUrl"] }
        };

    private static IStrategyResolver CreateStrategyResolver(IConfiguration config) =>
        new StrategyResolver(new Dictionary<OpponentType, IStrategy>
        {
            {
                OpponentType.Elon, StrategyResolverByName
                    .ResolveStrategyByName(config["Opponents:ElonStrategy"])
            },
            {
                OpponentType.Mark, StrategyResolverByName
                    .ResolveStrategyByName(config["Opponents:MarkStrategy"])
            }
        });
}