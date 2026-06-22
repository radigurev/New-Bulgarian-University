using MeepleNight.Domain.Enums;

namespace MeepleNight.Services.Dtos;

public class PlayNextSuggestionDto
{
    public Guid GameId { get; set; }
    public string Title { get; set; } = string.Empty;
    public GameCategory Category { get; set; }
    public int MinPlayers { get; set; }
    public int MaxPlayers { get; set; }
    public double Score { get; set; }
    public string Reason { get; set; } = string.Empty;
}

public class PlayNextResult
{
    public List<PlayNextSuggestionDto> Suggestions { get; set; } = new();
    public string? Reason { get; set; }
}
