using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DeckShufllerInterface;
using Strategy;

class Program
{

    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) => Host.CreateDefaultBuilder(args)
            .ConfigureServices((_, services) =>
            {
                services.AddHostedService<Gods>();
                services.AddScoped<Experiment>();
                services.AddScoped<IDeckShufller, DeckShufller>();
                services.AddScoped<ElonMusk>(provider => new(new ZeroStrategy()));
                services.AddScoped<MarkZuckerberg>(provider => new(new ZeroStrategy()));
            });
    }