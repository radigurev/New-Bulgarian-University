using MeepleNight.Domain.Enums;

namespace MeepleNight.Services.Dtos;

public class GameSummaryDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public GameCategory Category { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public int AverageDurationMinutes { get; set; }
    public string? CoverImagePath { get; set; }
    public bool IsActive { get; set; }
}

public class GameDetailsDto : GameSummaryDto
{
    public string? Description { get; set; }
    public int CommunitySessionCount { get; set; }
    public int? PersonalSessionCount { get; set; }
}

public class CreateGameRequest
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int MinPlayers { get; set; } = 2;
    public int MaxPlayers { get; set; } = 4;
    public int AverageDurationMinutes { get; set; } = 60;
    public GameCategory Category { get; set; } = GameCategory.Strategy;
    public string? CoverImagePath { get; set; }
}

public class UpdateGameRequest : CreateGameRequest
{
    public Guid Id { get; set; }
}

public class GameBrowseQuery
{
    public GameCategory? Category { get; set; }
    public int? PlayerCount { get; set; }
    public int? MaxDurationMinutes { get; set; }
    public string? Search { get; set; }
    public string Sort { get; set; } = "title";   // title | duration_asc | duration_desc | popularity
}
