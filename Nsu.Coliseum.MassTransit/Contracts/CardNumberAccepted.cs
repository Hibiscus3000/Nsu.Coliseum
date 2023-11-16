using MassTransit;
using ReposAndResolvers;

namespace Nsu.Coliseum.MassTransit.Contracts;

public class CardNumberAccepted
{
    public bool Success { get; init; }
    public MassTransitOpponentType OpponentType { get; init; }
    public long ExperimentNum { get; init; }
}

public class MassTransitOpponentType
{
    public OpponentType OpponentType { get; init; }
}