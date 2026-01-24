using System;
using System.Collections.Generic;
using FinCalc.Models;
using FinCalc.Services;

namespace FinCalc.Strategies;

public class LeasingCalculatorStrategy : ICalculationStrategy
{
    public CalculationKind Kind => CalculationKind.Leasing;

    public ICalculationResult Calculate(ICalculationRequest request)
    {
        if (request is not LeasingCalculationRequest leasingRequest)
        {
            throw new ArgumentException("Invalid request type for Leasing calculator.", nameof(request));
        }

        decimal financedAmount = leasingRequest.AssetPrice - leasingRequest.DownPayment;
        if (financedAmount <= 0m)
        {
            financedAmount = 0m;
        }

        decimal monthlyPaymentExcludingFee;

        if (financedAmount == 0m)
        {
            monthlyPaymentExcludingFee = 0m;
        }
        else
        {
            monthlyPaymentExcludingFee = FinancialMath.CalculateAnnuityPaymentWithBalloon(
                financedAmount,
                leasingRequest.AnnualInterestRatePercent,
                leasingRequest.TermMonths,
                leasingRequest.ResidualValue);
        }

        decimal monthlyInterestRate = leasingRequest.AnnualInterestRatePercent / 12m / 100m;
        decimal balance = FinancialMath.RoundMoney(financedAmount);

        List<AmortizationRow> schedule = new List<AmortizationRow>(capacity: leasingRequest.TermMonths);

        decimal totalInterest = 0m;
        decimal totalFees = 0m;

        DateOnly dueDate = leasingRequest.StartDate;

        for (int periodNumber = 1; periodNumber <= leasingRequest.TermMonths; periodNumber++)
        {
            decimal beginningBalance = balance;

            decimal interest = FinancialMath.RoundMoney(beginningBalance * monthlyInterestRate);
            decimal principalPaid = FinancialMath.RoundMoney(monthlyPaymentExcludingFee - interest);

            // For balloon structures, do not amortize below the residual value.
            decimal minimumBalanceAfterPayment = leasingRequest.ResidualValue;
            decimal maximumPrincipalThisPeriod = FinancialMath.RoundMoney(beginningBalance - minimumBalanceAfterPayment);

            if (maximumPrincipalThisPeriod < 0m)
            {
                maximumPrincipalThisPeriod = 0m;
            }

            if (principalPaid > maximumPrincipalThisPeriod)
            {
                principalPaid = maximumPrincipalThisPeriod;
            }

            if (principalPaid < 0m)
            {
                principalPaid = 0m;
            }

            decimal payment = FinancialMath.RoundMoney(principalPaid + interest);
            balance = FinancialMath.RoundMoney(balance - principalPaid);

            // Last-period correction for rounding: enforce ending balance == residual.
            if (periodNumber == leasingRequest.TermMonths)
            {
                decimal roundingDelta = FinancialMath.RoundMoney(balance - leasingRequest.ResidualValue);
                if (roundingDelta != 0m)
                {
                    decimal correctedPrincipal = FinancialMath.RoundMoney(principalPaid + roundingDelta);
                    if (correctedPrincipal >= 0m)
                    {
                        principalPaid = correctedPrincipal;
                        payment = FinancialMath.RoundMoney(principalPaid + interest);
                        balance = FinancialMath.RoundMoney(beginningBalance - principalPaid);
                    }
                }
            }

            decimal paymentWithFee = FinancialMath.RoundMoney(payment + leasingRequest.MonthlyFee);

            schedule.Add(new AmortizationRow(
                periodNumber,
                dueDate,
                beginningBalance,
                paymentWithFee,
                principalPaid,
                interest,
                balance));

            totalInterest = FinancialMath.RoundMoney(totalInterest + interest);
            totalFees = FinancialMath.RoundMoney(totalFees + leasingRequest.MonthlyFee);

            dueDate = dueDate.AddMonths(1);
        }

        totalFees = FinancialMath.RoundMoney(totalFees + leasingRequest.OneTimeFee);

        decimal totalPaid = FinancialMath.RoundMoney(
            leasingRequest.DownPayment +
            leasingRequest.OneTimeFee +
            (schedule.Count == 0 ? 0m : SumPayments(schedule)) +
            leasingRequest.ResidualValue);

        return new LeasingCalculationResult
        {
            MonthlyPayment = FinancialMath.RoundMoney(monthlyPaymentExcludingFee + leasingRequest.MonthlyFee),
            TotalPaid = totalPaid,
            TotalInterestPaid = totalInterest,
            TotalFeesPaid = totalFees,
            DownPayment = leasingRequest.DownPayment,
            ResidualValue = leasingRequest.ResidualValue,
            AmortizationSchedule = schedule
        };
    }

    private static decimal SumPayments(IReadOnlyList<AmortizationRow> schedule)
    {
        decimal sum = 0m;
        for (int index = 0; index < schedule.Count; index++)
        {
            sum = FinancialMath.RoundMoney(sum + schedule[index].Payment);
        }

        return sum;
    }
}
