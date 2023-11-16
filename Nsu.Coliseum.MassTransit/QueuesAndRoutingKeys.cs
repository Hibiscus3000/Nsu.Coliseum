using Nsu.Coliseum.Opponents;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit;

public enum QueueType
{
    Deck,
    CardNumber,
    CardNumberAccepted
}

public class MassTransitResolver<T>
{
    private readonly IDictionary<OpponentType, IDictionary<QueueType, T>> _opponentResolverDict
        = new Dictionary<OpponentType, IDictionary<QueueType, T>>();

    private readonly IDictionary<QueueType, T> _dict = new Dictionary<QueueType, T>();

    public void AddName(OpponentType opponentType, QueueType queueType, T name)
    {
        if (!_opponentResolverDict.ContainsKey(opponentType))
            _opponentResolverDict.Add(opponentType, new Dictionary<QueueType, T>());
        _opponentResolverDict[opponentType].Add(queueType, name);
    }

    public void AddName(QueueType queueType, T name) => _dict.Add(queueType, name);

    public T GetName(OpponentType opponentType, QueueType queueType) => _opponentResolverDict[opponentType][queueType];

    public T GetName(QueueType queueType) => _dict[queueType];
}

public static class QueuesAndRoutingKeys
{
    private const string DeckQueue = "Deck";
    private const string CardNumberQueue = "CardNumber";
    private const string CardNumberAcceptedQueue = "CardNumberAccepted";

    public static MassTransitResolver<QueueName> GetOpponentQueueNames(OpponentType opponentType)
    {
        var queueResolver = new MassTransitResolver<QueueName>();
        queueResolver.AddName(QueueType.Deck, new QueueName(DeckQueue + IOpponents.GetName(opponentType)));
        queueResolver.AddName(opponentType, QueueType.CardNumber, new QueueName(
            CardNumberQueue + IOpponents.GetName(opponentType)));
        queueResolver.AddName(IOpponents.GetOpposite(opponentType), QueueType.CardNumber, new QueueName(
            CardNumberQueue + IOpponents.GetName(IOpponents.GetOpposite(opponentType))));
        queueResolver.AddName(QueueType.CardNumberAccepted, new QueueName(CardNumberAcceptedQueue));
        return queueResolver;
    }

    public static MassTransitResolver<RoutingKey> GetOpponentRoutingKeys(OpponentType opponentType)
    {
        var routingKeyResolver = new MassTransitResolver<RoutingKey>();
        routingKeyResolver.AddName(QueueType.CardNumber, new RoutingKey(IOpponents.GetName(opponentType)));
        return routingKeyResolver;
    }

    public static MassTransitResolver<QueueName> GetMainQueueNames()
    {
        var queueResolver = new MassTransitResolver<QueueName>();
        foreach (OpponentType opponentType in Enum.GetValues(typeof(OpponentType)))
        {
            queueResolver.AddName(opponentType, QueueType.Deck,
                new QueueName(DeckQueue + IOpponents.GetName(opponentType)));
        }

        queueResolver.AddName(QueueType.CardNumberAccepted, new QueueName(CardNumberAcceptedQueue));
        return queueResolver;
    }
}

public class QueueName
{
    private const string Postfix = "";

    public string Value { get; }

    public QueueName(string value) => Value = value + Postfix;
}

public class RoutingKey
{
    public string Value { get; }

    public RoutingKey(string value) => Value = value;
}