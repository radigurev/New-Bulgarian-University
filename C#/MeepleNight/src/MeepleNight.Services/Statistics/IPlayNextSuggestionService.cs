using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Statistics;

public interface IPlayNextSuggestionService
{
    Task<PlayNextResult> SuggestAsync(int playerCount, CancellationToken ct = default);
}
