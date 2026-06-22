using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

/// <summary>Application user; extends ASP.NET Core Identity's user with display-level fields.</summary>
public class User : IdentityUser<Guid>
{
    [Required]
    [StringLength(50, MinimumLength = 2)]
    public string DisplayName { get; set; } = null!;

    [StringLength(260)]
    public string? AvatarPath { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? LastLoginAtUtc { get; set; }

    public ICollection<GameNight> HostedNights { get; set; } = new List<GameNight>();
    public ICollection<Invitation> Invitations { get; set; } = new List<Invitation>();
    public ICollection<SessionPlayer> SessionParticipations { get; set; } = new List<SessionPlayer>();
}
