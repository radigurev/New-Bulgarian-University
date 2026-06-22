using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

/// <summary>One played game during a GameNight.</summary>
public class Session
{
    public Guid Id { get; set; }

    [Required]
    public Guid GameNightId { get; set; }
    public GameNight? GameNight { get; set; }

    [Required]
    public Guid GameId { get; set; }
    public Game? Game { get; set; }

    [Required]
    public DateTime StartedAtUtc { get; set; }

    [Range(1, 720)]
    public int? DurationMinutes { get; set; }

    [StringLength(200)]
    public string? WinnerNote { get; set; }

    /// <summary>Required when the played Game.Category is Cooperative; null otherwise.</summary>
    public bool? IsCooperativeWin { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }

    public ICollection<SessionPlayer> Players { get; set; } = new List<SessionPlayer>();
}
