using System;

namespace FinCalc.Services;

public static class FinancialMath
{
    public static decimal CalculateAnnuityPayment(decimal principal, decimal annualInterestRatePercent, int termMonths)
    {
        if (termMonths <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(termMonths));
        }

        if (principal <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(principal));
        }

        decimal monthlyInterestRate = annualInterestRatePercent / 12m / 100m;

        if (monthlyInterestRate == 0m)
        {
            return RoundMoney(principal / termMonths);
        }

        decimal onePlusR = 1m + monthlyInterestRate;
        decimal denominator = 1m - (decimal)Math.Pow((double)onePlusR, -termMonths);

        decimal payment = principal * monthlyInterestRate / denominator;
        return RoundMoney(payment);
    }

    public static decimal CalculateAnnuityPaymentWithBalloon(
        decimal principal,
        decimal annualInterestRatePercent,
        int termMonths,
        decimal balloon)
    {
        if (termMonths <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(termMonths));
        }

        if (principal <= 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(principal));
        }

        if (balloon < 0m)
        {
            throw new ArgumentOutOfRangeException(nameof(balloon));
        }

        decimal monthlyInterestRate = annualInterestRatePercent / 12m / 100m;

        if (monthlyInterestRate == 0m)
        {
            // Straight-line principal amortization with a balloon.
            decimal amortizedPrincipal = principal - balloon;
            return RoundMoney(amortizedPrincipal / termMonths);
        }

        decimal onePlusR = 1m + monthlyInterestRate;
        decimal discountFactor = (decimal)Math.Pow((double)onePlusR, -termMonths);

        decimal presentValueOfBalloon = balloon * discountFactor;
        decimal adjustedPrincipal = principal - presentValueOfBalloon;

        decimal denominator = 1m - discountFactor;
        decimal payment = adjustedPrincipal * monthlyInterestRate / denominator;

        return RoundMoney(payment);
    }

    public static decimal RoundMoney(decimal value) => Math.Round(value, 2, MidpointRounding.AwayFromZero);
}
