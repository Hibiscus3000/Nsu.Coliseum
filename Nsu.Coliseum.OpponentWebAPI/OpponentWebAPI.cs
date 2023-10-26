using System.ComponentModel.DataAnnotations;
using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;

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
        // builder.Services.Configure<HostOptions>(opts =>
        //     opts.ShutdownTimeout = TimeSpan.FromSeconds(TimeoutSecs));

        var webStrategy = new WebStrategy();
        builder.Services
            .AddSingleton<WebStrategy>(_ => webStrategy);
        int strategyNameIndex = Array.IndexOf(args, "--strategy") + 1;
        if (0 != strategyNameIndex)
            webStrategy.Strategy = StrategyResolverByName.ResolveStrategyByName(args[strategyNameIndex]);

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

        app.Run();
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