using System.ComponentModel.DataAnnotations;

namespace FinCalc.Models;

public class RefinancingCalculationRequest : ICalculationRequest
{
    public CalculationKind Kind => CalculationKind.Refinancing;

    [Required]
    [Range(1, 1000000000)]
    [Display(Name = "Остатък по главница (лв.)")]
    public decimal ExistingRemainingPrincipal { get; set; } = 5000m;

    [Required]
    [Range(0, 100)]
    [Display(Name = "Текуща годишна лихва (%)")]
    public decimal ExistingAnnualInterestRatePercent { get; set; } = 10m;

    [Required]
    [Range(1, 600)]
    [Display(Name = "Оставащ срок (месеци)")]
    public int ExistingRemainingMonths { get; set; } = 36;

    [Range(0, 2000)]
    [Display(Name = "Текуща месечна такса (лв.)")]
    public decimal ExistingMonthlyFee { get; set; } = 0m;

    [Required]
    [Range(0, 100)]
    [Display(Name = "Нова годишна лихва (%)")]
    public decimal NewAnnualInterestRatePercent { get; set; } = 7m;

    [Required]
    [Range(1, 600)]
    [Display(Name = "Нов срок (месеци)")]
    public int NewTermMonths { get; set; } = 36;

    [Range(0, 1000000)]
    [Display(Name = "Еднократна такса за рефинансиране (лв.)")]
    public decimal NewOneTimeFee { get; set; } = 0m;

    [Range(0, 2000)]
    [Display(Name = "Нова месечна такса (лв.)")]
    public decimal NewMonthlyFee { get; set; } = 0m;
}
