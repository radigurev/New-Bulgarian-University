using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public interface IGameQueryService
{
    Task<List<GameSummaryDto>> BrowseAsync(GameBrowseQuery query, CancellationToken ct = default);

    Task<GameDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default);
}
