using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

/// <summary>A user's participation in a session, with their score and final placement.</summary>
public class SessionPlayer
{
    [Required]
    public Guid SessionId { get; set; }
    public Session? Session { get; set; }

    [Required]
    public Guid UserId { get; set; }
    public User? User { get; set; }

    public decimal? Score { get; set; }

    [Range(1, 100)]
    public int Placement { get; set; }
}
