using Nsu.Coliseum.Deck;
using Nsu.Coliseum.Opponents;
using Nsu.Coliseum.StrategyInterface;
using ReposAndResolvers;

namespace OpponentWebAPI;

public class WebOpponents : IOpponents
{
    private readonly IResolver<IStrategy> _strategyResolver;

    private readonly Dictionary<OpponentType, HttpClient> _clients = new();
    private const string ControllerPath = "/api/Opponent/";

    private bool _strategiesSend;

    private bool _appsChecked = false;
    private const int TryCount = 100;
    private const int WaitTimeoutMillis = 100;

    public WebOpponents(IResolver<IStrategy> strategyResolver, IResolver<OpponentUrl> urlResolver,
        bool needToSendStrategies = false)
    {
        _clients[OpponentType.Elon] = CreateHttpClient(urlResolver.GetT(OpponentType.Elon).Value);
        _clients[OpponentType.Mark] = CreateHttpClient(urlResolver.GetT(OpponentType.Mark).Value);

        _strategyResolver = strategyResolver;
        _strategiesSend = !needToSendStrategies;
    }

    private HttpClient CreateHttpClient(string url)
    {
        var httpClient = new HttpClient(CreateSocketHandler());
        httpClient.BaseAddress = new Uri(url);
        return httpClient;
    }

    private SocketsHttpHandler CreateSocketHandler() => new()
    {
        PooledConnectionLifetime = TimeSpan.FromMinutes(2)
    };

    private void SendStrategies()
    {
        foreach (OpponentType opponentType in Enum.GetValues(typeof(OpponentType)))
        {
            HttpResponseMessage responseMessage = SendStrategy(opponentType);
            if (responseMessage.IsSuccessStatusCode)
                throw new Exception($"Unable to send strategies to {opponentType}." +
                                    $"Status code: {responseMessage.StatusCode}.");
        }

        _strategiesSend = true;
    }

    private HttpResponseMessage SendStrategy(OpponentType opponentType) =>
        _clients[opponentType].PostAsJsonAsync(ControllerPath + "SetStrategy",
            _strategyResolver.GetT(opponentType)).Result;

    private void CheckApps()
    {
        foreach (OpponentType opponentType in Enum.GetValues(typeof(OpponentType))) CheckApp(opponentType);
        _appsChecked = true;
    }

    private void CheckApp(OpponentType opponentType)
    {
        for (int i = 0; i < TryCount; ++i)
        {
            try
            {
                if (_clients[opponentType].GetAsync("").Result.IsSuccessStatusCode) return;
            }
            catch (Exception e)
            {
            }

            Thread.Sleep(WaitTimeoutMillis);
        }

        throw new Exception($"Unable to establish connection to {opponentType} app.");
    }

    public Task<HttpResponseMessage> GetCardNumberResponseTask(OpponentType type, Card[] cards,
        string urlPath)
    {
        if (!_appsChecked) CheckApps();
        if (!_strategiesSend) SendStrategies();
        return _clients[type].PostAsJsonAsync(ControllerPath + urlPath,
            new WebDeck { Cards = cards });
    }

    private const string UseStrategyUrlPath = "UseStrategy";

    public int GetCardNumber(OpponentType type, Card[] cards) =>
        GetCardNumberResponseTask(type, cards, UseStrategyUrlPath)
            .Result
            .Content
            .ReadFromJsonAsync<int>()
            .Result;

    public async Task<int> GetCardNumberAsync(OpponentType type, Card[] cards)
    {
        HttpResponseMessage res = await GetCardNumberResponseTask(type, cards, UseStrategyUrlPath + "Async");
        int cardNumber = await res.Content.ReadFromJsonAsync<int>();
        return cardNumber;
    }
}