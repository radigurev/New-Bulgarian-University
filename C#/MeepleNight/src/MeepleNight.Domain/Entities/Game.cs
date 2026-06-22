using MeepleNight.Domain.Common;
using MeepleNight.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

public class Game : IAuditable, ISoftDeletable
{
    public Guid Id { get; set; }

    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = null!;

    [StringLength(2000)]
    public string? Description { get; set; }

    [Range(1, 20)]
    public int MinPlayers { get; set; }

    [Range(1, 20)]
    public int MaxPlayers { get; set; }

    [Range(5, 720)]
    public int AverageDurationMinutes { get; set; }

    [Required]
    public GameCategory Category { get; set; }

    [StringLength(260)]
    public string? CoverImagePath { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime CreatedAtUtc { get; set; }
    public Guid? CreatedByUserId { get; set; }
    public DateTime? UpdatedAtUtc { get; set; }
    public Guid? UpdatedByUserId { get; set; }

    public ICollection<Session> Sessions { get; set; } = new List<Session>();
    public ICollection<GameNightCandidate> Candidates { get; set; } = new List<GameNightCandidate>();
}
