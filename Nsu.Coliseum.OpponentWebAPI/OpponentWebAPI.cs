using System.ComponentModel.DataAnnotations;
using MassTransit;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.MassTransit.Consumers;
using Nsu.Coliseum.MassTransit.Contracts;
using Nsu.Coliseum.MassTransitOpponents;
using Nsu.Coliseum.Opponents;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;
using RabbitMQ.Client;
using ReposAndResolvers;

namespace OpponentWebAPI;

public static class OpponentWebApi
{
    private static readonly int TimeoutSecs;

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        if (Convert.ToBoolean(builder.Configuration["UseMassTransit"]))
            ConfigureMassTransit(builder.Configuration, builder.Services);

        ConfigureWebStrategy(builder.Configuration, builder.Services);

        // builder.Services.Configure<HostOptions>(opts =>
        //     opts.ShutdownTimeout = TimeSpan.FromSeconds(TimeoutSecs));

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.MapGet("/", () => Results.Ok());

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();
        // TODO try strartasync with cancellation token
        app.Run();
    }

    private static void ConfigureWebStrategy(IConfiguration config, IServiceCollection services)
    {
        var webStrategy = new WebStrategy();
        services.AddSingleton<WebStrategy>(_ => webStrategy);

        string? strategyFromConfig = config["Strategy"];
        if (null != strategyFromConfig)
            webStrategy.Strategy = StrategyResolverByName.ResolveStrategyByName(strategyFromConfig);
    }

    private static void ConfigureMassTransit(IConfiguration config, IServiceCollection services)
    {
        OpponentType opponentType = IOpponents.GetOpponentTypeByName(config["Name"]!);

        MassTransitResolver<QueueName> queues = QueuesAndRoutingKeys.GetOpponentQueueNames(opponentType);
        MassTransitResolver<RoutingKey> routingKeys = QueuesAndRoutingKeys.GetOpponentRoutingKeys(opponentType);

        services.AddSingleton<MassTransitOpponentType>(_ => new MassTransitOpponentType
            { OpponentType = opponentType });
        services.AddSingleton<MassTransitResolver<QueueName>>(_ => queues);
        services.AddSingleton<MassTransitResolver<RoutingKey>>(_ => routingKeys);

        services.AddSingleton<IRepo<CardColor>, Repo<CardColor>>();
        services.AddSingleton<IRepo<Card[]>, Repo<Card[]>>();
        services.AddSingleton<IStrategy>(_ => StrategyResolverByName.ResolveStrategyByName(config["Strategy"]!));

        AddMassTransit(opponentType, queues, routingKeys, services);
    }


    public static void AddMassTransit(OpponentType opponentType,
        MassTransitResolver<QueueName> queues,
        MassTransitResolver<RoutingKey> routingKeys,
        IServiceCollection services)
    {
        services.AddMassTransit(configurator =>
        {
            configurator.AddConsumer<PickCardFromDeckConsumer>();
            configurator.AddConsumer<CardPickedConsumer>();

            configurator.UsingRabbitMq((context, cfg) =>
            {
                cfg.Publish<CardNumberPicked>(m => m.ExchangeType = ExchangeType.Direct);
                cfg.ReceiveEndpoint(queues.GetName(QueueType.Deck).Value, e =>
                {
                    e.Lazy = true;
                    e.ExclusiveConsumer = true;

                    e.ConfigureConsumer<PickCardFromDeckConsumer>(context);
                });

                cfg.ReceiveEndpoint(queues.GetName(opponentType, QueueType.CardNumber).Value, e =>
                {
                    e.ConfigureConsumeTopology = false;
                    e.Lazy = true;
                    e.ExclusiveConsumer = true;

                    e.Bind<CardNumberPicked>(b =>
                    {
                        b.ExchangeType = ExchangeType.Direct;
                        b.RoutingKey = routingKeys.GetName(QueueType.CardNumber).Value;
                    });

                    e.ConfigureConsumer<CardPickedConsumer>(context);
                });
            });
        });
    }
}

public class WebStrategy
{
    [Required] public IStrategy? Strategy { get; set; } = null;
}

public class WebDeck
{
    private const int NumberOfCards = 18;

    [Required(ErrorMessage = "Cards are required")]
    [MinLength(NumberOfCards)]
    [MaxLength(NumberOfCards)]
    public Card[]? Cards { get; init; }
}