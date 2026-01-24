using System;
using System.ComponentModel.DataAnnotations;

namespace FinCalc.Models;

public class CreditCalculationRequest : ICalculationRequest
{
    public CalculationKind Kind => CalculationKind.Credit;

    [Required]
    [Range(1, 1000000000)]
    [Display(Name = "Главница (лв.)")]
    public decimal Principal { get; set; } = 10000m;

    [Required]
    [Range(0, 100)]
    [Display(Name = "Годишна лихва (%)")]
    public decimal AnnualInterestRatePercent { get; set; } = 8m;

    [Required]
    [Range(1, 600)]
    [Display(Name = "Срок (месеци)")]
    public int TermMonths { get; set; } = 60;

    [Required]
    [Display(Name = "Начална дата")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Range(0, 2000)]
    [Display(Name = "Месечна такса (лв.)")]
    public decimal MonthlyFee { get; set; } = 0m;
}
