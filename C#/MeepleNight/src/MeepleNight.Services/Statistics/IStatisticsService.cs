using MeepleNight.Services.Dtos;

namespace MeepleNight.Services.Statistics;

public interface IStatisticsService
{
    Task<StatisticsDashboardDto> GetForCurrentUserAsync(CancellationToken ct = default);
}
