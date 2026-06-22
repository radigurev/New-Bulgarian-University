using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Statistics;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using NUnit.Framework;

namespace MeepleNight.Tests.ServiceTests;

[TestFixture]
public sealed class PlayNextSuggestionServiceTests
{
    [Test]
    public async Task Suggest_FiltersByPlayerCount_OnlyEligibleGamesReturned()
    {
        Guid eligibleId = Guid.NewGuid();
        Guid tooBigId = Guid.NewGuid();
        var eligible = new List<Game>
        {
            new() { Id = eligibleId, Title = "Catan", IsActive = true, MinPlayers = 3, MaxPlayers = 4, AverageDurationMinutes = 90, Category = GameCategory.Strategy }
        };

        Mock<IGameRepository> gameRepo = new();
        gameRepo
            .Setup(r => r.ListActiveAsync(null, 4, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(eligible);

        Mock<ISessionRepository> sessionRepo = new();
        sessionRepo
            .Setup(r => r.GetForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Session>());

        Mock<ICurrentUserAccessor> currentUser = new();
        currentUser.SetupGet(u => u.UserId).Returns(Guid.NewGuid());

        PlayNextSuggestionService service = new(
            gameRepo.Object, sessionRepo.Object, currentUser.Object, NullLogger<PlayNextSuggestionService>.Instance);

        var result = await service.SuggestAsync(playerCount: 4);

        Assert.That(result.Suggestions, Has.Count.EqualTo(1));
        Assert.That(result.Suggestions[0].GameId, Is.EqualTo(eligibleId));
    }

    [Test]
    public async Task Suggest_NoEligibleGames_ReturnsEmptyWithReason()
    {
        Mock<IGameRepository> gameRepo = new();
        gameRepo
            .Setup(r => r.ListActiveAsync(null, It.IsAny<int>(), null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Game>());

        Mock<ISessionRepository> sessionRepo = new();
        Mock<ICurrentUserAccessor> currentUser = new();
        currentUser.SetupGet(u => u.UserId).Returns(Guid.NewGuid());

        PlayNextSuggestionService service = new(
            gameRepo.Object, sessionRepo.Object, currentUser.Object, NullLogger<PlayNextSuggestionService>.Instance);

        var result = await service.SuggestAsync(playerCount: 7);

        Assert.That(result.Suggestions, Is.Empty);
        Assert.That(result.Reason, Is.EqualTo("no_eligible_games"));
    }

    [Test]
    public async Task Suggest_NeverPlayedGame_GetsReason_NewToYou()
    {
        Guid gameId = Guid.NewGuid();
        var games = new List<Game>
        {
            new() { Id = gameId, Title = "Wingspan", IsActive = true, MinPlayers = 1, MaxPlayers = 5, AverageDurationMinutes = 70, Category = GameCategory.Strategy }
        };

        Mock<IGameRepository> gameRepo = new();
        gameRepo
            .Setup(r => r.ListActiveAsync(null, 4, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(games);

        Mock<ISessionRepository> sessionRepo = new();
        sessionRepo
            .Setup(r => r.GetForUserAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Session>());

        Mock<ICurrentUserAccessor> currentUser = new();
        currentUser.SetupGet(u => u.UserId).Returns(Guid.NewGuid());

        PlayNextSuggestionService service = new(
            gameRepo.Object, sessionRepo.Object, currentUser.Object, NullLogger<PlayNextSuggestionService>.Instance);

        var result = await service.SuggestAsync(4);

        Assert.That(result.Suggestions, Has.Count.EqualTo(1));
        Assert.That(result.Suggestions[0].Reason, Is.EqualTo("New to you"));
    }
}
