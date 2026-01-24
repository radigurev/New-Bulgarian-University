using FinCalc.Models;

namespace FinCalc.Services;

public interface ICalculationService
{
    ICalculationResult Calculate(ICalculationRequest request);
}
