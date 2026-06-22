using MeepleNight.Domain.Interfaces;
using MeepleNight.Services;
using MeepleNight.Services.Dtos;
using MeepleNight.Web.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MeepleNight.Web.Controllers;

[Authorize]
public class InvitationsController : Controller
{
    private readonly IInvitationService _invitations;
    private readonly IUserRepository _users;

    public InvitationsController(IInvitationService invitations, IUserRepository users)
    {
        _invitations = invitations;
        _users = users;
    }

    public async Task<IActionResult> Inbox(CancellationToken ct)
    {
        List<InboxInvitationDto> all = await _invitations.GetInboxAsync(ct);
        return View(InboxPageViewModel.From(all));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Send(SendInvitationsRequest form, CancellationToken ct)
    {
        Result<int> result = await _invitations.SendAsync(form, ct);
        if (result.IsSuccess)
        {
            TempData["Success"] = result.Value == 0
                ? "No new invitations to send."
                : $"Sent {result.Value} invitation(s).";
        }
        else
        {
            TempData["Error"] = result.Error ?? "Unable to send invitations.";
        }

        return RedirectToAction("Details", "GameNights", new { id = form.GameNightId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Respond(RespondToInvitationRequest form, CancellationToken ct)
    {
        Result result = await _invitations.RespondAsync(form, ct);
        if (result.IsSuccess) TempData["Success"] = "Response saved.";
        else TempData["Error"] = result.Error ?? "Unable to respond.";

        return RedirectToAction(nameof(Inbox));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Revoke(Guid invitationId, Guid gameNightId, CancellationToken ct)
    {
        Result result = await _invitations.RevokeAsync(invitationId, ct);
        if (result.IsSuccess) TempData["Success"] = "Invitation revoked.";
        else TempData["Error"] = result.Error ?? "Unable to revoke.";

        return RedirectToAction("Details", "GameNights", new { id = gameNightId });
    }

    [HttpGet]
    public async Task<IActionResult> SearchUsers(string q, CancellationToken ct)
    {
        var users = await _users.SearchAsync(q ?? string.Empty, 20, ct);
        return Json(users.Select(u => new { id = u.Id, displayName = u.DisplayName, email = u.Email }));
    }
}
