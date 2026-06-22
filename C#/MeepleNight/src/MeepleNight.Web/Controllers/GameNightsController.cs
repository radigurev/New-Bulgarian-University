using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using MeepleNight.Services.Statistics;
using MeepleNight.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Controllers;

[Authorize]
public class GameNightsController : Controller
{
    private readonly IGameNightService _nights;
    private readonly IGameAdminService _admin;
    private readonly IPlayNextSuggestionService _suggestions;

    public GameNightsController(
        IGameNightService nights,
        IGameAdminService admin,
        IPlayNextSuggestionService suggestions)
    {
        _nights = nights;
        _admin = admin;
        _suggestions = suggestions;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        GameNightListPageViewModel vm = new()
        {
            Hosting = await _nights.ListHostingAsync(ct),
            Invited = await _nights.ListInvitedAsync(ct)
        };
        return View(vm);
    }

    public async Task<IActionResult> Details(Guid id, CancellationToken ct)
    {
        GameNightDetailsDto? details = await _nights.GetDetailsAsync(id, ct);
        if (details == null) return NotFound();
        return View(details);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.AvailableGames = await _admin.ListAsync(null, ct);
        ViewBag.Suggestions = (await _suggestions.SuggestAsync(4, ct)).Suggestions;
        return View(new CreateGameNightRequest
        {
            ScheduledForUtc = DateTime.UtcNow.Date.AddDays(7).AddHours(19),
            ExpectedPlayerCount = 4
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGameNightRequest form, CancellationToken ct)
    {
        Result<Guid> result = await _nights.CreateAsync(form, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create.");
            ViewBag.AvailableGames = await _admin.ListAsync(null, ct);
            ViewBag.Suggestions = (await _suggestions.SuggestAsync(form.ExpectedPlayerCount, ct)).Suggestions;
            return View(form);
        }

        TempData["Success"] = $"Game night '{form.Title}' created.";
        return RedirectToAction(nameof(Details), new { id = result.Value });
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        GameNightDetailsDto? details = await _nights.GetDetailsAsync(id, ct);
        if (details == null) return NotFound();
        if (!details.ViewerIsHost) return Forbid();

        UpdateGameNightRequest form = new()
        {
            Id = details.Id,
            Title = details.Title,
            ScheduledForUtc = details.ScheduledForUtc,
            Location = details.Location,
            Notes = details.Notes,
            ExpectedPlayerCount = details.ExpectedPlayerCount,
            CandidateGameIds = details.CandidateGames.Select(c => c.GameId).ToList()
        };

        ViewBag.AvailableGames = await _admin.ListAsync(null, ct);
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateGameNightRequest form, CancellationToken ct)
    {
        Result result = await _nights.UpdateAsync(form, ct);
        if (!result.IsSuccess)
        {
            if (result.ErrorKind == ResultErrorKind.NotFound) return NotFound();
            if (result.ErrorKind == ResultErrorKind.Forbidden) return Forbid();
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to save.");
            ViewBag.AvailableGames = await _admin.ListAsync(null, ct);
            return View(form);
        }

        TempData["Success"] = "Game night saved.";
        return RedirectToAction(nameof(Details), new { id = form.Id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(Guid id, CancellationToken ct)
    {
        Result result = await _nights.CancelAsync(id, ct);
        if (result.IsSuccess) TempData["Success"] = "Game night cancelled.";
        else TempData["Error"] = result.Error ?? "Unable to cancel.";
        return RedirectToAction(nameof(Details), new { id });
    }
}
