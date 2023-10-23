using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Sandbox;
using OpponentWebAPI;

namespace OpponentWepAPI;

public class WebOpponents : IOpponents
{
    private readonly IStrategyResolver _strategyResolver;
    private bool _appsStarted = false;

    private readonly HttpClient _elonHttpClient;
    private readonly HttpClient _markHttpClient;

    private readonly string _elonName = "elon";
    private readonly string _markName = "mark";

    private readonly int _elonPort = 5219;
    private readonly int _markPort = 5220;

    public WebOpponents(IStrategyResolver strategyResolver)
    {
        _strategyResolver = strategyResolver;
        _elonHttpClient = new HttpClient(CreateSocketHandler());
        _markHttpClient = new HttpClient(CreateSocketHandler());

        //_elonHttpClient.BaseAddress = new Uri(OpponentWebApi.GetUrl(_elonName, _elonPort));
        //_markHttpClient.BaseAddress = new Uri(OpponentWebApi.GetUrl(_markName, _markPort));
    }

    private void StartApps()
    {
        WebApplication elonApp = OpponentWebApi.CreateApp(_strategyResolver.GetStrategy(OpponentType.Elon),
            opponentName: _elonName, port: _elonPort);
        WebApplication markApp = OpponentWebApi.CreateApp(_strategyResolver.GetStrategy(OpponentType.Mark),
            opponentName: _markName, port: _markPort);

        elonApp.RunAsync(url: OpponentWebApi.GetUrl(opponentName: _elonName, _elonPort));
        markApp.Run(url: OpponentWebApi.GetUrl(opponentName: _markName, _markPort));
        _appsStarted = true;
    }

    private SocketsHttpHandler CreateSocketHandler() => new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    };

    public int GetCardNumber(OpponentType type, Card[] cards)
    {
        if (!_appsStarted) StartApps();
        Task<HttpResponseMessage> response = (type switch
        {
            OpponentType.Elon => _elonHttpClient,
            OpponentType.Mark => _markHttpClient,
        }).PostAsJsonAsync("http://locahost:5219/Opponent/UseStrategy", cards);
        var res = response.GetAwaiter().GetResult();
        Console.WriteLine(res);
        return 0;
    }
}