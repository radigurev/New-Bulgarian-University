using MeepleNight.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

public class GameNight
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = null!;

    [Required]
    public DateTime ScheduledForUtc { get; set; }

    [Required]
    [StringLength(200, MinimumLength = 2)]
    public string Location { get; set; } = null!;

    [StringLength(1000)]
    public string? Notes { get; set; }

    [Range(2, 20)]
    public int ExpectedPlayerCount { get; set; }

    [Required]
    public Guid HostUserId { get; set; }
    public User? Host { get; set; }

    [Required]
    public GameNightStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public DateTime? CancelledAtUtc { get; set; }

    public ICollection<GameNightCandidate> Candidates { get; set; } = new List<GameNightCandidate>();
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    public ICollection<Session> Sessions { get; set; } = new List<Session>();
}
