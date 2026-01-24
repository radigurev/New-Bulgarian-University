using System;
using FinCalc.Models;
using FinCalc.Services;

namespace FinCalc.Strategies;

public class RefinancingCalculatorStrategy : ICalculationStrategy
{
    public CalculationKind Kind => CalculationKind.Refinancing;

    public ICalculationResult Calculate(ICalculationRequest request)
    {
        if (request is not RefinancingCalculationRequest refinancingRequest)
        {
            throw new ArgumentException("Invalid request type for Refinancing calculator.", nameof(request));
        }

        decimal existingMonthlyPaymentExcludingFee = FinancialMath.CalculateAnnuityPayment(
            refinancingRequest.ExistingRemainingPrincipal,
            refinancingRequest.ExistingAnnualInterestRatePercent,
            refinancingRequest.ExistingRemainingMonths);

        decimal existingMonthlyPayment = FinancialMath.RoundMoney(existingMonthlyPaymentExcludingFee + refinancingRequest.ExistingMonthlyFee);

        decimal existingTotalCost = FinancialMath.RoundMoney(existingMonthlyPayment * refinancingRequest.ExistingRemainingMonths);

        decimal newMonthlyPaymentExcludingFee = FinancialMath.CalculateAnnuityPayment(
            refinancingRequest.ExistingRemainingPrincipal,
            refinancingRequest.NewAnnualInterestRatePercent,
            refinancingRequest.NewTermMonths);

        decimal newMonthlyPayment = FinancialMath.RoundMoney(newMonthlyPaymentExcludingFee + refinancingRequest.NewMonthlyFee);

        decimal newTotalCost = FinancialMath.RoundMoney(refinancingRequest.NewOneTimeFee + (newMonthlyPayment * refinancingRequest.NewTermMonths));

        decimal totalSavings = FinancialMath.RoundMoney(existingTotalCost - newTotalCost);
        decimal monthlySavings = FinancialMath.RoundMoney(existingMonthlyPayment - newMonthlyPayment);

        int? breakEvenMonths = null;

        if (monthlySavings > 0m && refinancingRequest.NewOneTimeFee > 0m)
        {
            decimal rawMonths = refinancingRequest.NewOneTimeFee / monthlySavings;
            int months = (int)Math.Ceiling((double)rawMonths);
            breakEvenMonths = months <= 0 ? null : months;
        }

        return new RefinancingCalculationResult
        {
            ExistingMonthlyPayment = existingMonthlyPayment,
            ExistingTotalRemainingCost = existingTotalCost,
            NewMonthlyPayment = newMonthlyPayment,
            NewTotalCost = newTotalCost,
            TotalSavings = totalSavings,
            MonthlySavings = monthlySavings,
            BreakEvenMonths = breakEvenMonths
        };
    }
}
