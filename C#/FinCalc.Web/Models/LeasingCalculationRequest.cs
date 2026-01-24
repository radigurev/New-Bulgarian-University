using System;
using System.ComponentModel.DataAnnotations;

namespace FinCalc.Models;

public class LeasingCalculationRequest : ICalculationRequest
{
    public CalculationKind Kind => CalculationKind.Leasing;

    [Required]
    [Range(1, 1000000000)]
    [Display(Name = "Цена на актива (лв.)")]
    public decimal AssetPrice { get; set; } = 30000m;

    [Range(0, 1000000000)]
    [Display(Name = "Първоначална вноска (лв.)")]
    public decimal DownPayment { get; set; } = 3000m;

    [Range(0, 1000000000)]
    [Display(Name = "Остатъчна стойност (лв.)")]
    public decimal ResidualValue { get; set; } = 8000m;

    [Required]
    [Range(0, 100)]
    [Display(Name = "Годишна лихва (%)")]
    public decimal AnnualInterestRatePercent { get; set; } = 7m;

    [Required]
    [Range(1, 600)]
    [Display(Name = "Срок (месеци)")]
    public int TermMonths { get; set; } = 60;

    [Required]
    [Display(Name = "Начална дата")]
    public DateOnly StartDate { get; set; } = DateOnly.FromDateTime(DateTime.Today);

    [Range(0, 1000000)]
    [Display(Name = "Еднократна такса (лв.)")]
    public decimal OneTimeFee { get; set; } = 0m;

    [Range(0, 2000)]
    [Display(Name = "Месечна такса (лв.)")]
    public decimal MonthlyFee { get; set; } = 0m;
}
