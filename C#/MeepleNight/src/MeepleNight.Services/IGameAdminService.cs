using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public interface IGameAdminService
{
    Task<List<GameSummaryDto>> ListAsync(string? search, CancellationToken ct = default);

    Task<GameSummaryDto?> GetForEditAsync(Guid id, CancellationToken ct = default);

    Task<Result<Guid>> CreateAsync(CreateGameRequest request, CancellationToken ct = default);

    Task<Result> UpdateAsync(UpdateGameRequest request, CancellationToken ct = default);

    Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default);

    Task<Result> RestoreAsync(Guid id, CancellationToken ct = default);
}
