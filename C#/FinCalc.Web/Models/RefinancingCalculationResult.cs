namespace FinCalc.Models;

public class RefinancingCalculationResult : ICalculationResult
{
    public CalculationKind Kind => CalculationKind.Refinancing;

    public required decimal ExistingMonthlyPayment { get; init; }
    public required decimal ExistingTotalRemainingCost { get; init; }

    public required decimal NewMonthlyPayment { get; init; }
    public required decimal NewTotalCost { get; init; }

    public required decimal TotalSavings { get; init; }

    public required decimal MonthlySavings { get; init; }

    public int? BreakEvenMonths { get; init; }
}
