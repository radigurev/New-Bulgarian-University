using AutoMapper;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;

namespace MeepleNight.Services;

public sealed class GameQueryService : IGameQueryService
{
    private readonly IGameRepository _games;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IMapper _mapper;

    public GameQueryService(IGameRepository games, ICurrentUserAccessor currentUser, IMapper mapper)
    {
        _games = games;
        _currentUser = currentUser;
        _mapper = mapper;
    }

    public async Task<List<GameSummaryDto>> BrowseAsync(GameBrowseQuery query, CancellationToken ct = default)
    {
        List<Game> games = await _games.ListActiveAsync(
            query.Category, query.PlayerCount, query.MaxDurationMinutes, query.Search, ct);

        // Sorting (cheap, in-memory; lists are small for a friend-circle scale)
        IEnumerable<Game> ordered = query.Sort switch
        {
            "duration_asc" => games.OrderBy(g => g.AverageDurationMinutes),
            "duration_desc" => games.OrderByDescending(g => g.AverageDurationMinutes),
            "popularity" => games.OrderByDescending(g => g.Sessions.Count),
            _ => games.OrderBy(g => g.Title),
        };

        return _mapper.Map<List<GameSummaryDto>>(ordered.ToList());
    }

    public async Task<GameDetailsDto?> GetDetailsAsync(Guid id, CancellationToken ct = default)
    {
        Game? game = await _games.GetByIdAsync(id, includeInactive: _currentUser.IsAdmin, ct);
        if (game == null) return null;

        GameDetailsDto dto = _mapper.Map<GameDetailsDto>(game);
        dto.CommunitySessionCount = await _games.SessionCountAsync(id, ct);

        if (_currentUser.UserId.HasValue)
        {
            dto.PersonalSessionCount = await _games.SessionCountByUserAsync(id, _currentUser.UserId.Value, ct);
        }

        return dto;
    }
}
