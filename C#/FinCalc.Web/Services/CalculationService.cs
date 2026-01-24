using System;
using FinCalc.Models;
using FinCalc.Strategies;

namespace FinCalc.Services;

public class CalculationService : ICalculationService
{
    private readonly ICalculationStrategyFactory _strategyFactory;

    public CalculationService(ICalculationStrategyFactory strategyFactory)
    {
        _strategyFactory = strategyFactory ?? throw new ArgumentNullException(nameof(strategyFactory));
    }

    public ICalculationResult Calculate(ICalculationRequest request)
    {
        if (request is null)
        {
            throw new ArgumentNullException(nameof(request));
        }

        ICalculationStrategy strategy = _strategyFactory.GetStrategy(request.Kind);
        return strategy.Calculate(request);
    }
}
