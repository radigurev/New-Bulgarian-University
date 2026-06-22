using MeepleNight.Data;
using MeepleNight.Domain.Entities;
using MeepleNight.Domain.Enums;
using MeepleNight.Web.Infrastructure.Seeding.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MeepleNight.Web.Infrastructure.Seeding;

/// <summary>
/// Populates the database with illustrative development data — friend users,
/// a fleshed-out game catalogue, and a scenario of past/upcoming/cancelled
/// game nights with invitations and logged sessions. Runs once: if any
/// <see cref="GameNight"/> already exists the seeder is a no-op.
/// </summary>
public sealed class DevDataSeeder : IDevDataSeeder
{
    private const string DevPassword = "User123!";
    private const string UserRoleName = "User";

    private readonly MeepleDbContext _db;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<DevDataSeeder> _logger;

    /// <summary>Creates a new seeder bound to the active scope.</summary>
    public DevDataSeeder(
        MeepleDbContext db,
        UserManager<User> userManager,
        ILogger<DevDataSeeder> logger)
    {
        _db = db;
        _userManager = userManager;
        _logger = logger;
    }

    /// <inheritdoc />
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        bool alreadySeeded = await _db.GameNights.AnyAsync(cancellationToken);
        if (alreadySeeded)
        {
            _logger.LogInformation("DevDataSeeder skipped: game nights already exist.");
            return;
        }

        Dictionary<string, User> users = await SeedUsersAsync(cancellationToken);
        Dictionary<string, Game> games = await SeedGamesAsync(cancellationToken);

        DateTime now = DateTime.UtcNow;
        await BuildPastStrategyMarathonAsync(users, games, now, cancellationToken);
        await BuildPastCoopNightAsync(users, games, now, cancellationToken);
        await BuildPastPartyMixerAsync(users, games, now, cancellationToken);
        await BuildPastCatanShowdownAsync(users, games, now, cancellationToken);
        await BuildPastQuickPlaysAsync(users, games, now, cancellationToken);
        await BuildUpcomingFridayNightAsync(users, games, now, cancellationToken);
        await BuildUpcomingWeekendMarathonAsync(users, games, now, cancellationToken);
        await BuildCancelledNightAsync(users, games, now, cancellationToken);

        await _db.SaveChangesAsync(cancellationToken);
        _logger.LogInformation("DevDataSeeder finished seeding the development scenario.");
    }

    private async Task<Dictionary<string, User>> SeedUsersAsync(CancellationToken ct)
    {
        IReadOnlyList<DevUser> profiles =
        [
            new DevUser("anna",   "anna@meeplenight.local",   "Anna Smith"),
            new DevUser("boris",  "boris@meeplenight.local",  "Boris Petrov"),
            new DevUser("clara",  "clara@meeplenight.local",  "Clara Müller"),
            new DevUser("daniel", "daniel@meeplenight.local", "Daniel Lee"),
            new DevUser("emma",   "emma@meeplenight.local",   "Emma Johnson"),
        ];

        Dictionary<string, User> map = new(StringComparer.OrdinalIgnoreCase);

        User? admin = await _userManager.FindByEmailAsync("admin@meeplenight.local");
        if (admin != null) { map["admin"] = admin; }

        foreach (DevUser profile in profiles)
        {
            User user = await EnsureUserAsync(profile, ct);
            map[profile.Key] = user;
        }

        return map;
    }

    private async Task<User> EnsureUserAsync(DevUser profile, CancellationToken ct)
    {
        User? existing = await _userManager.FindByEmailAsync(profile.Email);
        if (existing != null) { return existing; }

        User user = new()
        {
            UserName = profile.Email,
            Email = profile.Email,
            EmailConfirmed = true,
            DisplayName = profile.DisplayName,
            CreatedAtUtc = DateTime.UtcNow,
        };

        IdentityResult create = await _userManager.CreateAsync(user, DevPassword);
        if (!create.Succeeded)
        {
            string errors = string.Join("; ", create.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not create dev user {profile.Email}: {errors}");
        }

        IdentityResult role = await _userManager.AddToRoleAsync(user, UserRoleName);
        if (!role.Succeeded)
        {
            string errors = string.Join("; ", role.Errors.Select(e => e.Description));
            throw new InvalidOperationException($"Could not add role to dev user {profile.Email}: {errors}");
        }

        return user;
    }

    private async Task<Dictionary<string, Game>> SeedGamesAsync(CancellationToken ct)
    {
        IReadOnlyList<DevGame> catalogue =
        [
            new DevGame("Catan",        "Classic resource-trading strategy.",                 3, 4, 90,  GameCategory.Strategy),
            new DevGame("Wingspan",     "Engine-builder about birds.",                        1, 5, 70,  GameCategory.Strategy),
            new DevGame("Codenames",    "Word-association party game for teams.",             2, 8, 30,  GameCategory.Party),
            new DevGame("Pandemic",     "Cooperative game; cure diseases together.",          2, 4, 60,  GameCategory.Cooperative),
            new DevGame("Dixit",        "Storytelling and abstract imagery.",                 3, 6, 45,  GameCategory.Family),
            new DevGame("Azul",         "Tile-laying abstract.",                              2, 4, 45,  GameCategory.Abstract),
            new DevGame("Carcassonne",  "Tile-laying medieval landscape builder.",            2, 5, 50,  GameCategory.Family),
            new DevGame("7 Wonders",    "Card-drafting civilization builder.",                3, 7, 40,  GameCategory.Strategy),
        ];

        Dictionary<string, Game> existing = await _db.Games
            .ToDictionaryAsync(g => g.Title, StringComparer.OrdinalIgnoreCase, ct);

        foreach (DevGame entry in catalogue)
        {
            if (existing.ContainsKey(entry.Title)) { continue; }

            Game game = new()
            {
                Title = entry.Title,
                Description = entry.Description,
                MinPlayers = entry.MinPlayers,
                MaxPlayers = entry.MaxPlayers,
                AverageDurationMinutes = entry.DurationMinutes,
                Category = entry.Category,
                IsActive = true,
                CreatedAtUtc = DateTime.UtcNow,
            };
            _db.Games.Add(game);
            existing[entry.Title] = game;
        }

        await _db.SaveChangesAsync(ct);
        return existing;
    }

    private Task BuildPastStrategyMarathonAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-21).Date.AddHours(19);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["admin"],
            Title = "Strategy Marathon",
            Location = "Admin's HQ",
            Notes = "Heavy strategy night to kick off the season.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 4,
            Status = GameNightStatus.Completed,
        });

        AddCandidate(night, games["Catan"], 1);
        AddCandidate(night, games["Wingspan"], 2);

        AddInvitation(night, users["anna"],  users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-3));
        AddInvitation(night, users["boris"], users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-3));
        AddInvitation(night, users["clara"], users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-2));

        Session catan = AddSession(night, games["Catan"], scheduled, 95);
        AddPlayer(catan, users["admin"], 1, 12m);
        AddPlayer(catan, users["anna"],  2, 10m);
        AddPlayer(catan, users["boris"], 3, 8m);
        AddPlayer(catan, users["clara"], 4, 7m);

        Session wingspan = AddSession(night, games["Wingspan"], scheduled.AddHours(2), 75);
        AddPlayer(wingspan, users["admin"], 2, 82m);
        AddPlayer(wingspan, users["anna"],  3, 75m);
        AddPlayer(wingspan, users["boris"], 1, 95m);
        AddPlayer(wingspan, users["clara"], 4, 60m);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildPastCoopNightAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-14).Date.AddHours(20);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["anna"],
            Title = "Co-op Night",
            Location = "Anna's place",
            Notes = "Bring snacks. We're saving the world.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 4,
            Status = GameNightStatus.Completed,
        });

        AddCandidate(night, games["Pandemic"], 1);

        AddInvitation(night, users["admin"], users["anna"], InvitationStatus.Accepted, scheduled.AddDays(-2));
        AddInvitation(night, users["boris"], users["anna"], InvitationStatus.Accepted, scheduled.AddDays(-2));
        AddInvitation(night, users["clara"], users["anna"], InvitationStatus.Accepted, scheduled.AddDays(-1));

        Session pandemic = AddSession(night, games["Pandemic"], scheduled, 65, isCoopWin: true,
            winnerNote: "Cured all four diseases on the final turn.");
        AddPlayer(pandemic, users["admin"], 1, null);
        AddPlayer(pandemic, users["anna"],  1, null);
        AddPlayer(pandemic, users["boris"], 1, null);
        AddPlayer(pandemic, users["clara"], 1, null);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildPastPartyMixerAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-10).Date.AddHours(19);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["clara"],
            Title = "Party Games Mixer",
            Location = "The Board Game Café",
            Notes = "Casual party games — newcomers welcome.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 6,
            Status = GameNightStatus.Completed,
        });

        AddCandidate(night, games["Codenames"], 1);
        AddCandidate(night, games["Dixit"], 2);

        AddInvitation(night, users["admin"],  users["clara"], InvitationStatus.Accepted, scheduled.AddDays(-2));
        AddInvitation(night, users["anna"],   users["clara"], InvitationStatus.Accepted, scheduled.AddDays(-2));
        AddInvitation(night, users["boris"],  users["clara"], InvitationStatus.Accepted, scheduled.AddDays(-1));
        AddInvitation(night, users["daniel"], users["clara"], InvitationStatus.Declined, scheduled.AddDays(-1));
        AddInvitation(night, users["emma"],   users["clara"], InvitationStatus.Accepted, scheduled.AddDays(-1));

        Session codenames = AddSession(night, games["Codenames"], scheduled, 35);
        AddPlayer(codenames, users["admin"], 2, null);
        AddPlayer(codenames, users["anna"],  1, null);
        AddPlayer(codenames, users["boris"], 3, null);
        AddPlayer(codenames, users["clara"], 4, null);
        AddPlayer(codenames, users["emma"],  2, null);

        Session dixit = AddSession(night, games["Dixit"], scheduled.AddHours(1), 50);
        AddPlayer(dixit, users["admin"], 1, 30m);
        AddPlayer(dixit, users["anna"],  2, 25m);
        AddPlayer(dixit, users["boris"], 3, 20m);
        AddPlayer(dixit, users["clara"], 4, 18m);
        AddPlayer(dixit, users["emma"],  5, 15m);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildPastCatanShowdownAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-4).Date.AddHours(19);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["admin"],
            Title = "Catan Showdown",
            Location = "Admin's HQ",
            Notes = null,
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 4,
            Status = GameNightStatus.Completed,
        });

        AddCandidate(night, games["Catan"], 1);

        AddInvitation(night, users["boris"],  users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-1));
        AddInvitation(night, users["clara"],  users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-1));
        AddInvitation(night, users["daniel"], users["admin"], InvitationStatus.Accepted, scheduled.AddDays(-1));

        Session catan = AddSession(night, games["Catan"], scheduled, 100,
            winnerNote: "Last-turn longest road steal.");
        AddPlayer(catan, users["admin"],  1, 14m);
        AddPlayer(catan, users["boris"],  2, 11m);
        AddPlayer(catan, users["clara"],  3, 9m);
        AddPlayer(catan, users["daniel"], 4, 7m);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildPastQuickPlaysAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-2).Date.AddHours(20);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["boris"],
            Title = "Quick Plays",
            Location = "Boris's apartment",
            Notes = "Short evening — two games tops.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 3,
            Status = GameNightStatus.Completed,
        });

        AddCandidate(night, games["Azul"], 1);
        AddCandidate(night, games["Catan"], 2);

        AddInvitation(night, users["admin"], users["boris"], InvitationStatus.Accepted, scheduled.AddDays(-1));
        AddInvitation(night, users["anna"],  users["boris"], InvitationStatus.Accepted, scheduled.AddDays(-1));

        Session azul = AddSession(night, games["Azul"], scheduled, 45);
        AddPlayer(azul, users["admin"], 2, 56m);
        AddPlayer(azul, users["anna"],  3, 48m);
        AddPlayer(azul, users["boris"], 1, 72m);

        Session catan = AddSession(night, games["Catan"], scheduled.AddHours(1), 80);
        AddPlayer(catan, users["admin"], 2, 8m);
        AddPlayer(catan, users["anna"],  3, 7m);
        AddPlayer(catan, users["boris"], 1, 11m);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildUpcomingFridayNightAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(3).Date.AddHours(19);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["admin"],
            Title = "Friday Strategy Night",
            Location = "Admin's HQ",
            Notes = "Bring a notebook. We're trying Wingspan with the expansion.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 5,
            Status = GameNightStatus.Planned,
        });

        AddCandidate(night, games["Catan"], 1);
        AddCandidate(night, games["Wingspan"], 2);
        AddCandidate(night, games["Azul"], 3);

        AddInvitation(night, users["anna"],   users["admin"], InvitationStatus.Accepted, now.AddDays(-1));
        AddInvitation(night, users["boris"],  users["admin"], InvitationStatus.Accepted, now.AddDays(-1));
        AddInvitation(night, users["clara"],  users["admin"], InvitationStatus.Pending,  respondedAtUtc: null);
        AddInvitation(night, users["daniel"], users["admin"], InvitationStatus.Pending,  respondedAtUtc: null);
        AddInvitation(night, users["emma"],   users["admin"], InvitationStatus.Pending,  respondedAtUtc: null);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildUpcomingWeekendMarathonAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(10).Date.AddHours(15);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["anna"],
            Title = "Weekend Marathon",
            Location = "Anna's place",
            Notes = "All-afternoon session. Lunch provided.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 4,
            Status = GameNightStatus.Planned,
        });

        AddCandidate(night, games["Wingspan"], 1);
        AddCandidate(night, games["Pandemic"], 2);
        AddCandidate(night, games["7 Wonders"], 3);

        AddInvitation(night, users["admin"], users["anna"], InvitationStatus.Pending,  respondedAtUtc: null);
        AddInvitation(night, users["boris"], users["anna"], InvitationStatus.Accepted, now);
        AddInvitation(night, users["clara"], users["anna"], InvitationStatus.Accepted, now);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private Task BuildCancelledNightAsync(
        Dictionary<string, User> users,
        Dictionary<string, Game> games,
        DateTime now,
        CancellationToken ct)
    {
        DateTime scheduled = now.AddDays(-5).Date.AddHours(20);
        DateTime cancelled = scheduled.AddDays(-1);
        GameNight night = CreateNight(new NightInputs
        {
            Host = users["boris"],
            Title = "Storm Cancellation",
            Location = "Boris's apartment",
            Notes = "Snowstorm hit — rescheduling.",
            ScheduledForUtc = scheduled,
            ExpectedPlayerCount = 3,
            Status = GameNightStatus.Cancelled,
            CancelledAtUtc = cancelled,
        });

        AddCandidate(night, games["Codenames"], 1);

        AddInvitation(night, users["admin"], users["boris"], InvitationStatus.Revoked, cancelled);
        AddInvitation(night, users["anna"],  users["boris"], InvitationStatus.Revoked, cancelled);

        _db.GameNights.Add(night);
        return Task.CompletedTask;
    }

    private static GameNight CreateNight(NightInputs inputs)
    {
        return new GameNight
        {
            Id = Guid.NewGuid(),
            Title = inputs.Title,
            Location = inputs.Location,
            Notes = inputs.Notes,
            ScheduledForUtc = inputs.ScheduledForUtc,
            ExpectedPlayerCount = inputs.ExpectedPlayerCount,
            HostUserId = inputs.Host.Id,
            Status = inputs.Status,
            CreatedAtUtc = inputs.ScheduledForUtc.AddDays(-7),
            CancelledAtUtc = inputs.CancelledAtUtc,
        };
    }

    private static void AddCandidate(GameNight night, Game game, int position)
    {
        night.Candidates.Add(new GameNightCandidate
        {
            GameNightId = night.Id,
            GameId = game.Id,
            Position = position,
        });
    }

    private static void AddInvitation(
        GameNight night,
        User invitee,
        User invitedBy,
        InvitationStatus status,
        DateTime? respondedAtUtc)
    {
        night.Invitations.Add(new Invitation
        {
            Id = Guid.NewGuid(),
            GameNightId = night.Id,
            InviteeUserId = invitee.Id,
            InvitedByUserId = invitedBy.Id,
            Status = status,
            CreatedAtUtc = night.CreatedAtUtc,
            RespondedAtUtc = respondedAtUtc,
        });
    }

    private static Session AddSession(
        GameNight night,
        Game game,
        DateTime startedAtUtc,
        int durationMinutes,
        bool? isCoopWin = null,
        string? winnerNote = null)
    {
        Session session = new()
        {
            Id = Guid.NewGuid(),
            GameNightId = night.Id,
            GameId = game.Id,
            StartedAtUtc = startedAtUtc,
            DurationMinutes = durationMinutes,
            IsCooperativeWin = isCoopWin,
            WinnerNote = winnerNote,
            CreatedAtUtc = startedAtUtc,
        };
        night.Sessions.Add(session);
        return session;
    }

    private static void AddPlayer(Session session, User user, int placement, decimal? score)
    {
        session.Players.Add(new SessionPlayer
        {
            SessionId = session.Id,
            UserId = user.Id,
            Placement = placement,
            Score = score,
        });
    }

    /// <summary>Descriptor used to build the night entity from a single configuration block.</summary>
    private sealed class NightInputs
    {
        public required User Host { get; init; }
        public required string Title { get; init; }
        public required string Location { get; init; }
        public string? Notes { get; init; }
        public required DateTime ScheduledForUtc { get; init; }
        public required int ExpectedPlayerCount { get; init; }
        public required GameNightStatus Status { get; init; }
        public DateTime? CancelledAtUtc { get; init; }
    }

    /// <summary>Compact descriptor of a dev user to be created.</summary>
    private sealed class DevUser
    {
        public DevUser(string key, string email, string displayName)
        {
            Key = key;
            Email = email;
            DisplayName = displayName;
        }
        public string Key { get; }
        public string Email { get; }
        public string DisplayName { get; }
    }

    /// <summary>Compact descriptor of a dev game to be created if missing.</summary>
    private sealed class DevGame
    {
        public DevGame(string title, string description, int min, int max, int duration, GameCategory category)
        {
            Title = title;
            Description = description;
            MinPlayers = min;
            MaxPlayers = max;
            DurationMinutes = duration;
            Category = category;
        }
        public string Title { get; }
        public string Description { get; }
        public int MinPlayers { get; }
        public int MaxPlayers { get; }
        public int DurationMinutes { get; }
        public GameCategory Category { get; }
    }
}
