using System.Collections.Generic;

namespace FinCalc.Models;

public class LeasingCalculationResult : ICalculationResult
{
    public CalculationKind Kind => CalculationKind.Leasing;

    public required decimal MonthlyPayment { get; init; }

    public required decimal TotalPaid { get; init; }

    public required decimal TotalInterestPaid { get; init; }

    public required decimal TotalFeesPaid { get; init; }

    public required decimal DownPayment { get; init; }

    public required decimal ResidualValue { get; init; }

    public required IReadOnlyList<AmortizationRow> AmortizationSchedule { get; init; }
}
