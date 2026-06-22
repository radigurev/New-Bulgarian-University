using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public interface IInvitationService
{
    Task<List<InboxInvitationDto>> GetInboxAsync(CancellationToken ct = default);

    Task<int> GetPendingCountAsync(CancellationToken ct = default);

    Task<Result<int>> SendAsync(SendInvitationsRequest request, CancellationToken ct = default);

    Task<Result> RespondAsync(RespondToInvitationRequest request, CancellationToken ct = default);

    Task<Result> RevokeAsync(Guid invitationId, CancellationToken ct = default);
}
