using System;

namespace FinCalc.Models;

public class AmortizationRow
{
    public AmortizationRow(
        int periodNumber,
        DateOnly dueDate,
        decimal beginningBalance,
        decimal payment,
        decimal principalPaid,
        decimal interestPaid,
        decimal endingBalance)
    {
        PeriodNumber = periodNumber;
        DueDate = dueDate;
        BeginningBalance = beginningBalance;
        Payment = payment;
        PrincipalPaid = principalPaid;
        InterestPaid = interestPaid;
        EndingBalance = endingBalance;
    }

    public int PeriodNumber { get; }

    public DateOnly DueDate { get; }

    public decimal BeginningBalance { get; }

    public decimal Payment { get; }

    public decimal PrincipalPaid { get; }

    public decimal InterestPaid { get; }

    public decimal EndingBalance { get; }
}
