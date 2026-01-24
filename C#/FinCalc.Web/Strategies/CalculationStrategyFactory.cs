using System;
using System.Collections.Generic;
using System.Linq;
using FinCalc.Models;

namespace FinCalc.Strategies;

public class CalculationStrategyFactory : ICalculationStrategyFactory
{
    private readonly IReadOnlyDictionary<CalculationKind, ICalculationStrategy> _strategiesByKind;

    public CalculationStrategyFactory(IEnumerable<ICalculationStrategy> strategies)
    {
        if (strategies is null)
        {
            throw new ArgumentNullException(nameof(strategies));
        }

        _strategiesByKind = strategies.ToDictionary(strategy => strategy.Kind, strategy => strategy);
    }

    public ICalculationStrategy GetStrategy(CalculationKind kind)
    {
        if (!_strategiesByKind.TryGetValue(kind, out ICalculationStrategy? strategy))
        {
            throw new InvalidOperationException($"No calculation strategy registered for kind '{kind}'.");
        }

        return strategy;
    }
}
