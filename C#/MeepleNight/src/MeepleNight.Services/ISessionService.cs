using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public interface ISessionService
{
    Task<Result<Guid>> LogAsync(LogSessionRequest request, CancellationToken ct = default);

    Task<Result> DeleteAsync(Guid sessionId, CancellationToken ct = default);
}
