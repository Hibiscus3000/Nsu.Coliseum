using Nsu.Coliseum.StrategyInterface;

namespace Nsu.Coliseum.Strategies;

public static class StrategyResolverByName
{
    public static IStrategy ResolveStrategyByName(string strategyName) => strategyName switch
    {
        "zero-strategy" => new ZeroStrategy(),
        _ => throw new ArgumentException($"Unknown strategy name: {strategyName}")
    };
}