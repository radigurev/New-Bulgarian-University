using FinCalc.Models;

namespace FinCalc.Strategies;

public interface ICalculationStrategyFactory
{
    ICalculationStrategy GetStrategy(CalculationKind kind);
}
