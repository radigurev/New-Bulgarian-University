using MeepleNight.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace MeepleNight.Domain.Entities;

public class Invitation
{
    public Guid Id { get; set; }

    [Required]
    public Guid GameNightId { get; set; }
    public GameNight? GameNight { get; set; }

    [Required]
    public Guid InviteeUserId { get; set; }
    public User? Invitee { get; set; }

    [Required]
    public Guid InvitedByUserId { get; set; }
    public User? InvitedBy { get; set; }

    [Required]
    public InvitationStatus Status { get; set; }

    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
}
