using MeepleNight.Domain.Enums;
using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Controllers;

[AllowAnonymous]
public class GamesController : Controller
{
    private readonly IGameQueryService _games;

    public GamesController(IGameQueryService games)
    {
        _games = games;
    }

    [HttpGet]
    public async Task<IActionResult> Index(
        GameCategory? category,
        int? players,
        int? maxDuration,
        string? search,
        string? sort,
        CancellationToken ct)
    {
        GameBrowseQuery query = new()
        {
            Category = category,
            PlayerCount = players,
            MaxDurationMinutes = maxDuration,
            Search = search,
            Sort = sort ?? "title"
        };

        ViewBag.Query = query;
        List<GameSummaryDto> list = await _games.BrowseAsync(query, ct);
        return View(list);
    }

    [HttpGet]
    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        GameDetailsDto? dto = await _games.GetDetailsAsync(id, ct);
        if (dto == null) return NotFound();
        return View(dto);
    }
}
