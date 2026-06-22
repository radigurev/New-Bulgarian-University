using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using MeepleNight.Services.Statistics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Controllers;

[Authorize]
public class StatisticsController : Controller
{
    private readonly IStatisticsService _stats;
    private readonly IPlayNextSuggestionService _suggestions;

    public StatisticsController(IStatisticsService stats, IPlayNextSuggestionService suggestions)
    {
        _stats = stats;
        _suggestions = suggestions;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        StatisticsDashboardDto dashboard = await _stats.GetForCurrentUserAsync(ct);
        return View(dashboard);
    }

    // JSON endpoint consumed by the Game Night create form
    [HttpGet("api/playnext")]
    [Produces("application/json")]
    public async Task<IActionResult> PlayNext(int players = 4, CancellationToken ct = default)
    {
        PlayNextResult result = await _suggestions.SuggestAsync(players, ct);
        return Ok(result);
    }
}
