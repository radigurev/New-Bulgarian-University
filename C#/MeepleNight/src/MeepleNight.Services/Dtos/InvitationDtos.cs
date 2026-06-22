using MeepleNight.Domain.Enums;

namespace MeepleNight.Services.Dtos;

public class InboxInvitationDto
{
    public Guid InvitationId { get; set; }
    public Guid GameNightId { get; set; }
    public string GameNightTitle { get; set; } = string.Empty;
    public DateTime ScheduledForUtc { get; set; }
    public string Location { get; set; } = string.Empty;
    public string HostDisplayName { get; set; } = string.Empty;
    public InvitationStatus Status { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public DateTime? RespondedAtUtc { get; set; }
}

public class SendInvitationsRequest
{
    public Guid GameNightId { get; set; }
    public List<Guid> InviteeUserIds { get; set; } = new();
}

public class RespondToInvitationRequest
{
    public Guid InvitationId { get; set; }
    public InvitationStatus Response { get; set; }   // Accepted or Declined
}
