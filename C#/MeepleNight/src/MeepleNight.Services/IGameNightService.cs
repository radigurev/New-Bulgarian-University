using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public interface IGameNightService
{
    Task<List<GameNightSummaryDto>> ListHostingAsync(CancellationToken ct = default);

    Task<List<GameNightSummaryDto>> ListInvitedAsync(CancellationToken ct = default);

    Task<GameNightDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default);

    Task<Result<Guid>> CreateAsync(CreateGameNightRequest request, CancellationToken ct = default);

    Task<Result> UpdateAsync(UpdateGameNightRequest request, CancellationToken ct = default);

    Task<Result> CancelAsync(Guid id, CancellationToken ct = default);
}
