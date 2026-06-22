using MeepleNight.Services.Dtos;

namespace MeepleNight.Web.Models.ViewModels;

public class GameNightListPageViewModel
{
    public List<GameNightSummaryDto> Hosting { get; set; } = new();
    public List<GameNightSummaryDto> Invited { get; set; } = new();
}
