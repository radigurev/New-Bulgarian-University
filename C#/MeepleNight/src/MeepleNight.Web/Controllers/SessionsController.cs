using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Controllers;

[Authorize]
public class SessionsController : Controller
{
    private readonly ISessionService _sessions;
    private readonly IGameNightService _nights;

    public SessionsController(ISessionService sessions, IGameNightService nights)
    {
        _sessions = sessions;
        _nights = nights;
    }

    [HttpGet]
    public async Task<IActionResult> Log(Guid gameNightId, CancellationToken ct)
    {
        GameNightDetailsDto? details = await _nights.GetDetailsAsync(gameNightId, ct);
        if (details == null) return NotFound();
        if (!details.ViewerIsHost) return Forbid();

        ViewBag.Night = details;

        return View(new LogSessionRequest
        {
            GameNightId = gameNightId,
            StartedAtUtc = DateTime.UtcNow,
            GameId = details.CandidateGames.FirstOrDefault()?.GameId ?? Guid.Empty
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Log(LogSessionRequest form, CancellationToken ct)
    {
        Result<Guid> result = await _sessions.LogAsync(form, ct);
        if (!result.IsSuccess)
        {
            ModelState.AddModelError(string.Empty, result.Error ?? "Unable to log session.");
            GameNightDetailsDto? details = await _nights.GetDetailsAsync(form.GameNightId, ct);
            ViewBag.Night = details;
            return View(form);
        }

        TempData["Success"] = "Session logged.";
        return RedirectToAction("Details", "GameNights", new { id = form.GameNightId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id, Guid gameNightId, CancellationToken ct)
    {
        Result result = await _sessions.DeleteAsync(id, ct);
        if (result.IsSuccess) TempData["Success"] = "Session removed.";
        else TempData["Error"] = result.Error ?? "Unable to remove session.";

        return RedirectToAction("Details", "GameNights", new { id = gameNightId });
    }
}
