using FluentValidation;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Validators;

public sealed class CreateGameNightRequestValidator : AbstractValidator<CreateGameNightRequest>
{
    public CreateGameNightRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty().MinimumLength(2).MaximumLength(100);

        RuleFor(r => r.Location)
            .NotEmpty().MinimumLength(2).MaximumLength(200);

        RuleFor(r => r.Notes)
            .MaximumLength(1000);

        RuleFor(r => r.ExpectedPlayerCount)
            .InclusiveBetween(2, 20);

        RuleFor(r => r.ScheduledForUtc)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-1))
            .WithMessage("Game night must be scheduled in the future.");

        RuleFor(r => r.CandidateGameIds)
            .Must(ids => ids == null || ids.Distinct().Count() == ids.Count)
            .WithMessage("Candidate games must be distinct.")
            .Must(ids => ids == null || ids.Count <= 10)
            .WithMessage("At most 10 candidate games are allowed.");
    }
}

public sealed class UpdateGameNightRequestValidator : AbstractValidator<UpdateGameNightRequest>
{
    public UpdateGameNightRequestValidator()
    {
        Include(new CreateGameNightRequestValidator());
        RuleFor(r => r.Id).NotEmpty();
    }
}
