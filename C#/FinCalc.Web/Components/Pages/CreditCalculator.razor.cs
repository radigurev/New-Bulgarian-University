using System;
using System.Collections.Generic;
using System.Globalization;

using FinCalc.Models;
using FinCalc.Services;

using Microsoft.AspNetCore.Components;

namespace FinCalc.Components.Pages;

public partial class CreditCalculator
{
    [Inject]
    public required ICalculationService CalculationService { get; set; }

    private CreditCalculationRequest Request { get; } = new CreditCalculationRequest();

    private CreditCalculationResult? Result { get; set; }

    private bool ShowFullSchedule { get; set; }

    private IReadOnlyList<AmortizationRow> VisibleSchedule
    {
        get
        {
            if (Result is null)
            {
                return Array.Empty<AmortizationRow>();
            }

            if (ShowFullSchedule)
            {
                return Result.AmortizationSchedule;
            }

            int takeCount = Math.Min(12, Result.AmortizationSchedule.Count);
            List<AmortizationRow> subset = new List<AmortizationRow>(takeCount);
            for (int index = 0; index < takeCount; index++)
            {
                subset.Add(Result.AmortizationSchedule[index]);
            }

            return subset;
        }
    }

    private DateTime StartDate
    {
        get => Request.StartDate.ToDateTime(TimeOnly.MinValue);
        set => Request.StartDate = DateOnly.FromDateTime(value);
    }

    private void OnCalculate()
    {
        ICalculationResult result = CalculationService.Calculate(Request);
        Result = (CreditCalculationResult)result;
    }

    private static string FormatMoney(decimal value) => string.Format(CultureInfo.CurrentCulture, "{0:C2}", value);
}
