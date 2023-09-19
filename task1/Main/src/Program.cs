using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DeckShufllerInterface;
using Strategy;
using Sandbox;

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
                services.AddScoped<Gods.IGodsConfig, Gods.DefaultGodsConfig>(provider => {
                    int numberOfExperiments;
                    int numberOfCards;

                    if (args.Length >= 1) 
                    {
                        int.TryParse(args[0], out numberOfExperiments);   
                        if (args.Length >= 2)
                        {
                            int.TryParse(args[1], out numberOfCards);
                        }
                    }

                    // defaults
                    numberOfExperiments = 1_000_000;
                    numberOfCards = 36;

                    return new(numberOfExperiments, numberOfCards);
                });
                services.AddScoped<Experiment>();
                services.AddScoped<IDeckShufller, DeckShufller>();
                services.AddScoped<ElonMusk>(provider => new(new ZeroStrategy()));
                services.AddScoped<MarkZuckerberg>(provider => new(new ZeroStrategy()));
            });
    }