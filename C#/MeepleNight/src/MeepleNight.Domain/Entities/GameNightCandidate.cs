using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

/// <summary>Join row between a GameNight and a candidate Game (the host's shortlist).</summary>
public class GameNightCandidate
{
    [Required]
    public Guid GameNightId { get; set; }
    public GameNight? GameNight { get; set; }

    [Required]
    public Guid GameId { get; set; }
    public Game? Game { get; set; }

    [Range(1, 10)]
    public int Position { get; set; }
}
