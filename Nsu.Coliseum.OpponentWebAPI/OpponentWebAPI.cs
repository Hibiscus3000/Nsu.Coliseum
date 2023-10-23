using Nsu.Coliseum.Strategies;
using Nsu.Coliseum.StrategyInterface;

namespace OpponentWebAPI;

public static class OpponentWebApi
{
    public static void Main(string[] args)
    {
        CreateApp(new ZeroStrategy(), "localhost", 5219, args).RunAsync(GetUrl("localhost", 5219));
        CreateApp(new ZeroStrategy(), "localhost", 5220, args).Run(GetUrl("localhost", 5220));
    }

    public static WebApplication CreateApp(IStrategy strategy, string opponentName, int port,
        string[]? args = null)
    {
        var builder = WebApplication.CreateBuilder(args ?? Array.Empty<string>());

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddScoped<IStrategy>(_ => strategy);

        WebApplication app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        return app;
    }

    public static string GetUrl(string opponentName, int port)
    {
        return $"http://localhost:{port}";
    }
}