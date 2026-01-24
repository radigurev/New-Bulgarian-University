using System;
using System.Collections.Generic;
using FinCalc.Models;
using FinCalc.Services;

namespace FinCalc.Strategies;

public class CreditCalculatorStrategy : ICalculationStrategy
{
    public CalculationKind Kind => CalculationKind.Credit;

    public ICalculationResult Calculate(ICalculationRequest request)
    {
        if (request is not CreditCalculationRequest creditRequest)
        {
            throw new ArgumentException("Invalid request type for Credit calculator.", nameof(request));
        }

        decimal monthlyPaymentExcludingFee = FinancialMath.CalculateAnnuityPayment(
            creditRequest.Principal,
            creditRequest.AnnualInterestRatePercent,
            creditRequest.TermMonths);

        decimal monthlyInterestRate = creditRequest.AnnualInterestRatePercent / 12m / 100m;
        decimal balance = FinancialMath.RoundMoney(creditRequest.Principal);

        List<AmortizationRow> schedule = new List<AmortizationRow>(capacity: creditRequest.TermMonths);

        decimal totalInterest = 0m;
        decimal totalFees = 0m;
        decimal totalPaid = 0m;

        DateOnly dueDate = creditRequest.StartDate;

        for (int periodNumber = 1; periodNumber <= creditRequest.TermMonths; periodNumber++)
        {
            decimal beginningBalance = balance;

            decimal interest = FinancialMath.RoundMoney(beginningBalance * monthlyInterestRate);
            decimal principalPaid = FinancialMath.RoundMoney(monthlyPaymentExcludingFee - interest);

            if (principalPaid > balance)
            {
                principalPaid = balance;
            }

            decimal payment = FinancialMath.RoundMoney(principalPaid + interest);

            balance = FinancialMath.RoundMoney(balance - principalPaid);

            decimal paymentWithFee = FinancialMath.RoundMoney(payment + creditRequest.MonthlyFee);

            schedule.Add(new AmortizationRow(
                periodNumber,
                dueDate,
                beginningBalance,
                paymentWithFee,
                principalPaid,
                interest,
                balance));

            totalInterest = FinancialMath.RoundMoney(totalInterest + interest);
            totalFees = FinancialMath.RoundMoney(totalFees + creditRequest.MonthlyFee);
            totalPaid = FinancialMath.RoundMoney(totalPaid + paymentWithFee);

            dueDate = dueDate.AddMonths(1);
        }

        return new CreditCalculationResult
        {
            MonthlyPayment = FinancialMath.RoundMoney(monthlyPaymentExcludingFee + creditRequest.MonthlyFee),
            TotalPaid = totalPaid,
            TotalInterestPaid = totalInterest,
            TotalFeesPaid = totalFees,
            AmortizationSchedule = schedule
        };
    }
}
