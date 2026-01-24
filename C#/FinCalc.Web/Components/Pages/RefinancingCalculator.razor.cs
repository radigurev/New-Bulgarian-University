using System.Globalization;

using FinCalc.Models;
using FinCalc.Services;

using Microsoft.AspNetCore.Components;

namespace FinCalc.Components.Pages;

public partial class RefinancingCalculator
{
    [Inject]
    public required ICalculationService CalculationService { get; set; }

    private RefinancingCalculationRequest Request { get; } = new RefinancingCalculationRequest();

    private RefinancingCalculationResult? Result { get; set; }

    private void OnCalculate()
    {
        ICalculationResult result = CalculationService.Calculate(Request);
        Result = (RefinancingCalculationResult)result;
    }

    private static string FormatMoney(decimal value) => string.Format(CultureInfo.CurrentCulture, "{0:C2}", value);
}
