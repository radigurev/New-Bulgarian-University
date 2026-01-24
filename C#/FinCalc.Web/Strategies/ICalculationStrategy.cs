using FinCalc.Models;

namespace FinCalc.Strategies;

public interface ICalculationStrategy
{
    CalculationKind Kind { get; }

    ICalculationResult Calculate(ICalculationRequest request);
}
