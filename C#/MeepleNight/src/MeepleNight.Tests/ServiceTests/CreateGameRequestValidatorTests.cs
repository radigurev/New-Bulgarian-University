using MeepleNight.Domain.Enums;
using MeepleNight.Services.Dtos;
using MeepleNight.Services.Validators;
using NUnit.Framework;

namespace MeepleNight.Tests.ServiceTests;

[TestFixture]
public sealed class CreateGameRequestValidatorTests
{
    private CreateGameRequestValidator _validator = null!;

    [SetUp]
    public void Setup()
    {
        _validator = new CreateGameRequestValidator();
    }

    [Test]
    public void Validate_HappyPath_PassesValidation()
    {
        CreateGameRequest request = new()
        {
            Title = "Catan",
            MinPlayers = 3,
            MaxPlayers = 4,
            AverageDurationMinutes = 90,
            Category = GameCategory.Strategy
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.True);
    }

    [Test]
    public void Validate_MaxPlayersLessThanMin_Fails()
    {
        CreateGameRequest request = new()
        {
            Title = "Bad Game",
            MinPlayers = 6,
            MaxPlayers = 4,
            AverageDurationMinutes = 60,
            Category = GameCategory.Strategy
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateGameRequest.MaxPlayers)), Is.True);
    }

    [Test]
    public void Validate_DurationOutOfRange_Fails()
    {
        CreateGameRequest request = new()
        {
            Title = "Slow Game",
            MinPlayers = 2,
            MaxPlayers = 4,
            AverageDurationMinutes = 1000,
            Category = GameCategory.Strategy
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateGameRequest.AverageDurationMinutes)), Is.True);
    }

    [Test]
    public void Validate_EmptyTitle_Fails()
    {
        CreateGameRequest request = new()
        {
            Title = "",
            MinPlayers = 2,
            MaxPlayers = 4,
            AverageDurationMinutes = 60,
            Category = GameCategory.Strategy
        };

        var result = _validator.Validate(request);

        Assert.That(result.IsValid, Is.False);
        Assert.That(result.Errors.Any(e => e.PropertyName == nameof(CreateGameRequest.Title)), Is.True);
    }
}
