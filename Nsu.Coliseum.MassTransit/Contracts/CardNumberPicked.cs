using MassTransit;

namespace Nsu.Coliseum.MassTransit.Contracts
{
    public record CardNumberPicked
    {
        public int CardNumber { get; init; }
        public long ExperimentNum { get; init; }
    }
}