using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nsu.Coliseum.Database;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Opponents;
using Nsu.Coliseum.Sandbox;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;
using OpponentWebAPI;
using ReposAndResolvers;

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

                ConfigureDeckSizeAndExperimentsNum(config, services);
                services.AddScoped<IExperimentContext, ExperimentContext>();
                ConfigureExperimentRunner(config, services);
                ConfigureDeckProvider(config, services);
                ConfigureUrlResolver(config, services);
                ConfigureStrategyResolver(config, services);
                ConfigureOpponents(config, services);
            });
    }

    private static void ConfigureDeckSizeAndExperimentsNum(IConfiguration config,
        IServiceCollection services) =>
        services.AddScoped<DeckSizeAndNumOfDecks>(_ => new DeckSizeAndNumOfDecks
        {
            NumberOfCardsInDeck = null != config["NumberOfCards"] &&
                                  int.TryParse(config["NumberOfCards"], out int resultNC)
                ? resultNC
                : 36,
            NumberOfDecks = null != config["NumberOfExperiments"] &&
                            int.TryParse(config["NumberOfExperiments"], out int resultNE)
                ? resultNE
                : 36
        });

    private static void ConfigureDeckProvider(IConfiguration config,
        IServiceCollection services)
    {
        switch (config["DeckProvider"])
        {
            case "random":
                services.AddScoped<IDeckProvider, RandomDeckProvider>();
                break;
            case "database":
                services.AddScoped<IDeckProvider, DbDeckProvider>();
                break;
        }
    }

    private static void ConfigureExperimentRunner(IConfiguration config,
        IServiceCollection services)
    {
        switch (config["ExperimentRunnerType"])
        {
            case "default":
                services.AddScoped<IExperimentRunner, ExperimentRunner>();
                break;
            case "async":
                services.AddScoped<IExperimentRunner, ExperimentRunnerAsync>();
                break;
            // case "mass-transit":
            //     services.AddScoped<IExperimentRunner, MassTransitExperimentRunner>();
            //     services.AddMassTransit(configurator =>
            //     {
            //         configurator.UsingRabbitMq((context, cfg) =>
            //         {
            //             cfg.ConfigureEndpoints(context);
            //             cfg.ReceiveEndpoint("elon-decks", e =>
            //             {
            //                 e.Lazy = true;
            //                 e.ExclusiveConsumer = true;
            //                 //TODO PrefetchCount
            //                 e.ConfigureConsumer<DeckAndCardNumberConsumer>(context);
            //             });
            //         });
            //     });
            //     ConfigureDeckQueueNameResolver(config, services);
            //     
            //     break;
        }
    }

    private static void ConfigureOpponents(IConfiguration config,
        IServiceCollection services)
    {
        switch (config["Opponents:Type"])
        {
            case "web":
                services.AddScoped<IOpponents, WebOpponents>();
                break;
            case "default":
                services.AddScoped<IOpponents, Opponents.Opponents>();
                break;
        }
    }

    private static void ConfigureUrlResolver(IConfiguration config,
        IServiceCollection services)
    {
        var urlResolver = new Resolver<OpponentUrl>();
        urlResolver.SaveT(OpponentType.Elon, new OpponentUrl { Value = config["Opponents:ElonUrl"] });
        urlResolver.SaveT(OpponentType.Mark, new OpponentUrl { Value = config["Opponents:MarkUrl"] });

        services.AddScoped<IResolver<OpponentUrl>>(_ => urlResolver);
    }

    private static void ConfigureStrategyResolver(IConfiguration config,
        IServiceCollection services)
    {
        Resolver<IStrategy> strategyResolver = new Resolver<IStrategy>();
        strategyResolver.SaveT(OpponentType.Elon, StrategyResolverByName
            .ResolveStrategyByName(config["Opponents:ElonStrategy"]));
        strategyResolver.SaveT(OpponentType.Mark, StrategyResolverByName
            .ResolveStrategyByName(config["Opponents:MarkStrategy"]));

        services.AddScoped<IResolver<IStrategy>>(_ => strategyResolver);
    }

    // private static void ConfigureDeckQueueNameResolver(IConfiguration config,
    //     IServiceCollection services)
    // {
    //     var queueResolver = new Resolver<QueueName>();
    //     queueResolver.SaveT(OpponentType.Elon, new QueueName{Value = config["Queues:ElonDecks"]});
    //     queueResolver.SaveT(OpponentType.Mark, new QueueName{Value = config["Queues:MarkDecks"]});
    //     services.AddScoped<IResolver<QueueName>>(_ => queueResolver);
    // }
}