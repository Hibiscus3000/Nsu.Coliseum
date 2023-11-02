using MassTransit;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Contracts;

public class CardNumberAccepted : CorrelatedBy<Guid>
{
    public bool Success { get; init; }
    public MassTransitOpponentType OpponentType { get; init; }
    public Guid CorrelationId { get; init; }
}

public class MassTransitOpponentType
{
    public OpponentType OpponentType { get; init; }
}