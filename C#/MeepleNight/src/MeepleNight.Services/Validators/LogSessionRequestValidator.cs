using FluentValidation;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Validators;

public sealed class LogSessionRequestValidator : AbstractValidator<LogSessionRequest>
{
    public LogSessionRequestValidator()
    {
        RuleFor(r => r.GameNightId).NotEmpty();
        RuleFor(r => r.GameId).NotEmpty();

        RuleFor(r => r.StartedAtUtc)
            .LessThanOrEqualTo(DateTime.UtcNow.AddHours(1))
            .WithMessage("Session start time cannot be in the far future.");

        RuleFor(r => r.DurationMinutes)
            .InclusiveBetween(1, 720)
            .When(r => r.DurationMinutes.HasValue);

        RuleFor(r => r.WinnerNote)
            .MaximumLength(200);

        RuleFor(r => r.Players)
            .NotNull()
            .Must(p => p != null && p.Count >= 2)
            .WithMessage("A session must have at least 2 players.")
            .Must(p => p == null || p.Select(x => x.UserId).Distinct().Count() == p.Count)
            .WithMessage("Each player can only appear once in a session.");

        RuleForEach(r => r.Players).ChildRules(player =>
        {
            player.RuleFor(p => p.UserId).NotEmpty();
            player.RuleFor(p => p.Placement).GreaterThanOrEqualTo(1);
            player.RuleFor(p => p.Score).InclusiveBetween(-9999, 9999).When(p => p.Score.HasValue);
        });
    }
}
