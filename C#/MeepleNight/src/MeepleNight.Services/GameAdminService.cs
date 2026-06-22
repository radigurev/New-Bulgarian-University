using AutoMapper;
using FluentValidation;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Interfaces;
using MeepleNight.Services.Dtos;
using Microsoft.Extensions.Logging;

namespace MeepleNight.Services;

public sealed class GameAdminService : IGameAdminService
{
    private readonly IGameRepository _games;
    private readonly ICurrentUserAccessor _currentUser;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateGameRequest> _createValidator;
    private readonly IValidator<UpdateGameRequest> _updateValidator;
    private readonly ILogger<GameAdminService> _logger;

    public GameAdminService(
        IGameRepository games,
        ICurrentUserAccessor currentUser,
        IMapper mapper,
        IValidator<CreateGameRequest> createValidator,
        IValidator<UpdateGameRequest> updateValidator,
        ILogger<GameAdminService> logger)
    {
        _games = games;
        _currentUser = currentUser;
        _mapper = mapper;
        _createValidator = createValidator;
        _updateValidator = updateValidator;
        _logger = logger;
    }

    public async Task<List<GameSummaryDto>> ListAsync(string? search, CancellationToken ct = default)
    {
        List<Game> games = await _games.ListAllForAdminAsync(search, ct);
        return _mapper.Map<List<GameSummaryDto>>(games);
    }

    public async Task<GameSummaryDto?> GetForEditAsync(Guid id, CancellationToken ct = default)
    {
        Game? game = await _games.GetByIdAsync(id, includeInactive: true, ct);
        return game == null ? null : _mapper.Map<GameSummaryDto>(game);
    }

    public async Task<Result<Guid>> CreateAsync(CreateGameRequest request, CancellationToken ct = default)
    {
        var validation = await _createValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return Result<Guid>.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        if (await _games.TitleExistsActiveAsync(request.Title, excludeId: null, ct))
        {
            return Result<Guid>.Conflict("A game with this title already exists.");
        }

        Game game = _mapper.Map<Game>(request);
        game.CreatedByUserId = _currentUser.UserId;

        await _games.AddAsync(game, ct);
        await _games.SaveChangesAsync(ct);

        _logger.LogInformation("Admin {AdminId} created game {GameId} ({Title})", _currentUser.UserId, game.Id, game.Title);
        return Result<Guid>.Ok(game.Id);
    }

    public async Task<Result> UpdateAsync(UpdateGameRequest request, CancellationToken ct = default)
    {
        var validation = await _updateValidator.ValidateAsync(request, ct);
        if (!validation.IsValid)
        {
            return Result.Fail(string.Join("; ", validation.Errors.Select(e => e.ErrorMessage)));
        }

        Game? game = await _games.GetByIdAsync(request.Id, includeInactive: true, ct);
        if (game == null) return Result.NotFound();

        if (await _games.TitleExistsActiveAsync(request.Title, excludeId: request.Id, ct))
        {
            return Result.Conflict("A game with this title already exists.");
        }

        game.Title = request.Title;
        game.Description = request.Description;
        game.MinPlayers = request.MinPlayers;
        game.MaxPlayers = request.MaxPlayers;
        game.AverageDurationMinutes = request.AverageDurationMinutes;
        game.Category = request.Category;
        game.CoverImagePath = request.CoverImagePath;
        game.UpdatedByUserId = _currentUser.UserId;

        _games.Update(game);
        await _games.SaveChangesAsync(ct);

        _logger.LogInformation("Admin {AdminId} updated game {GameId}", _currentUser.UserId, game.Id);
        return Result.Ok();
    }

    public async Task<Result> DeactivateAsync(Guid id, CancellationToken ct = default)
    {
        Game? game = await _games.GetByIdAsync(id, includeInactive: true, ct);
        if (game == null) return Result.NotFound();
        if (!game.IsActive) return Result.Ok();   // already deactivated

        game.IsActive = false;
        game.UpdatedByUserId = _currentUser.UserId;
        _games.Update(game);
        await _games.SaveChangesAsync(ct);

        _logger.LogInformation("Admin {AdminId} deactivated game {GameId}", _currentUser.UserId, game.Id);
        return Result.Ok();
    }

    public async Task<Result> RestoreAsync(Guid id, CancellationToken ct = default)
    {
        Game? game = await _games.GetByIdAsync(id, includeInactive: true, ct);
        if (game == null) return Result.NotFound();
        if (game.IsActive) return Result.Ok();

        if (await _games.TitleExistsActiveAsync(game.Title, excludeId: id, ct))
        {
            return Result.Conflict("Another active game with the same title already exists; rename or delete it first.");
        }

        game.IsActive = true;
        game.UpdatedByUserId = _currentUser.UserId;
        _games.Update(game);
        await _games.SaveChangesAsync(ct);

        _logger.LogInformation("Admin {AdminId} restored game {GameId}", _currentUser.UserId, game.Id);
        return Result.Ok();
    }
}
