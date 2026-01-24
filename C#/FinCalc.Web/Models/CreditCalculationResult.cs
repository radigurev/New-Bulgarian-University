using System.Collections.Generic;

namespace FinCalc.Models;

public class CreditCalculationResult : ICalculationResult
{
    public CalculationKind Kind => CalculationKind.Credit;

    public required decimal MonthlyPayment { get; init; }

    public required decimal TotalPaid { get; init; }

    public required decimal TotalInterestPaid { get; init; }

    public required decimal TotalFeesPaid { get; init; }

    public required IReadOnlyList<AmortizationRow> AmortizationSchedule { get; init; }
}
