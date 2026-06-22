using FluentValidation;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Validators;

public sealed class CreateGameRequestValidator : AbstractValidator<CreateGameRequest>
{
    public CreateGameRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(r => r.Description)
            .MaximumLength(2000);

        RuleFor(r => r.MinPlayers)
            .GreaterThanOrEqualTo(1);

        RuleFor(r => r.MaxPlayers)
            .GreaterThanOrEqualTo(r => r.MinPlayers)
            .WithMessage("Max players must be at least the value of Min players.")
            .LessThanOrEqualTo(20);

        RuleFor(r => r.AverageDurationMinutes)
            .InclusiveBetween(5, 720);

        RuleFor(r => r.CoverImagePath)
            .MaximumLength(260);
    }
}

public sealed class UpdateGameRequestValidator : AbstractValidator<UpdateGameRequest>
{
    public UpdateGameRequestValidator()
    {
        Include(new CreateGameRequestValidator());
        RuleFor(r => r.Id).NotEmpty();
    }
}
