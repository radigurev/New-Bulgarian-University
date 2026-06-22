using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Policy = "RequireAdmin")]
public class GamesController : Controller
{
    private readonly IGameAdminService _admin;

    public GamesController(IGameAdminService admin)
    {
        _admin = admin;
    }

    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        ViewBag.Search = search;
        List<GameSummaryDto> list = await _admin.ListAsync(search, ct);
        return View(list);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View(new CreateGameRequest());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CreateGameRequest form, CancellationToken ct)
    {
        Result<Guid> result = await _admin.CreateAsync(form, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to create game.");
            return View(form);
        }

        TempData["Success"] = $"Game '{form.Title}' added.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(Guid id, CancellationToken ct)
    {
        GameSummaryDto? game = await _admin.GetForEditAsync(id, ct);
        if (game == null) return NotFound();

        UpdateGameRequest form = new()
        {
            Id = game.Id,
            Title = game.Title,
            MinPlayers = game.MinPlayers,
            MaxPlayers = game.MaxPlayers,
            AverageDurationMinutes = game.AverageDurationMinutes,
            Category = game.Category,
            CoverImagePath = game.CoverImagePath
        };
        return View(form);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UpdateGameRequest form, CancellationToken ct)
    {
        Result result = await _admin.UpdateAsync(form, ct);
        if (!result.IsSuccess)
        {
            if (result.ErrorKind == ResultErrorKind.NotFound) return NotFound();
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to save.");
            return View(form);
        }

        TempData["Success"] = "Game saved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Deactivate(Guid id, CancellationToken ct)
    {
        Result result = await _admin.DeactivateAsync(id, ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Game deactivated."
            : (result.Error ?? "Unable to deactivate.");
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Restore(Guid id, CancellationToken ct)
    {
        Result result = await _admin.RestoreAsync(id, ct);
        TempData[result.IsSuccess ? "Success" : "Error"] = result.IsSuccess
            ? "Game restored."
            : (result.Error ?? "Unable to restore.");
        return RedirectToAction(nameof(Index));
    }
}
