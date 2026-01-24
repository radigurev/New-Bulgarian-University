namespace FinCalc.Models;

public interface ICalculationRequest
{
    CalculationKind Kind { get; }
}
