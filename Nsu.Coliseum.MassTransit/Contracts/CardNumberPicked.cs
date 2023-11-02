using MassTransit;

namespace Nsu.Coliseum.MassTransit.Contracts
{
    public record CardNumberPicked : CorrelatedBy<Guid>
    {
        public int CardNumber { get; init; }
        public Guid CorrelationId { get; init; }
    }
}