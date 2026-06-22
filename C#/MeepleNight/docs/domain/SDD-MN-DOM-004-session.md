---
id: SDD-MN-DOM-004
title: Session & SessionPlayer
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-006
  - SDD-MN-CORE-007
  - SDD-MN-CORE-008
  - SDD-MN-DOM-002
  - SDD-MN-DOM-003
---

# SDD-MN-DOM-004 — Session & SessionPlayer

## 1. Context

A `Session` represents one actual play of one game during a `GameNight`. A
`SessionPlayer` represents one player's participation in that session, with their
score and final placement.

Sessions are the source of truth for personal statistics (SDD-MN-CORE-007) and
the play-next suggestion engine (SDD-MN-CORE-008).

**Key Files (planned):**
- `src/MeepleNight.Domain/Entities/Session.cs`
- `src/MeepleNight.Domain/Entities/SessionPlayer.cs`
- `src/MeepleNight.Data/Configurations/SessionConfiguration.cs`
- `src/MeepleNight.Data/Configurations/SessionPlayerConfiguration.cs`

## 2. Properties — Session

| Property | Type | Required | Notes |
|---|---|---|---|
| Id | Guid | Yes | PK, `NEWSEQUENTIALID()` |
| GameNightId | Guid | Yes | FK to GameNight |
| GameId | Guid | Yes | FK to Game |
| StartedAtUtc | datetime2(7) | Yes | When the session started |
| DurationMinutes | int? | No | 1..720 |
| WinnerNote | string(200) | No | Free text — e.g. "Tied between A and B" |
| IsCooperativeWin | bool? | No | Required iff `Game.Category = Cooperative`; otherwise null |
| CreatedAtUtc | datetime2(7) | Yes | Default `SYSUTCDATETIME()` |
| UpdatedAtUtc | datetime2(7)? | No | |

## 3. Properties — SessionPlayer

| Property | Type | Required | Notes |
|---|---|---|---|
| SessionId | Guid | Yes | Composite PK part 1, FK to Session |
| UserId | Guid | Yes | Composite PK part 2, FK to User |
| Score | decimal(9,2)? | No | –9999.99 .. 9999.99 |
| Placement | int | Yes | ≥ 1; ties allowed |

## 4. Relationships

| Relationship | Cardinality | Notes |
|---|---|---|
| Session → GameNight | * → 1 | `OnDelete(Cascade)` from parent |
| Session → Game | * → 1 | `OnDelete(Restrict)` |
| Session → SessionPlayer | 1 → * | `OnDelete(Cascade)` |
| SessionPlayer → User | * → 1 | `OnDelete(Restrict)` |

## 5. Invariants

- Each `Session` **MUST** have ≥ 2 `SessionPlayer` rows.
- `SessionPlayer` rows of the same `Session` **MUST** have distinct `UserId`s.
- Each player **MUST** be either the host of the parent night or an attendee
  with `Invitation.Status = Accepted` for that night. (Validated in service
  layer; no DB-level check.)
- For non-cooperative games, **at least one** `SessionPlayer` **MUST** have
  `Placement = 1`.
- For cooperative games (`Game.Category = Cooperative`):
  - `IsCooperativeWin` **MUST** be non-null on the Session.
  - All `SessionPlayer` rows **MUST** share the same `Placement` value (typically `1`).

## 6. Indexes (for stats performance)

- `IX_SessionPlayers_UserId` on `SessionPlayer.UserId`
- `IX_Sessions_GameId` on `Session.GameId`
- `IX_Sessions_GameNightId` on `Session.GameNightId`
- Composite `IX_SessionPlayers_UserId_Placement` for "win count" rollups

## 7. EF Core Configuration

```
// Session
builder.ToTable("Sessions");
builder.HasKey(s => s.Id);
builder.Property(s => s.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
builder.Property(s => s.StartedAtUtc).HasColumnType("datetime2(7)");
builder.Property(s => s.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
builder.Property(s => s.UpdatedAtUtc).HasColumnType("datetime2(7)");
builder.Property(s => s.WinnerNote).HasMaxLength(200);
builder.HasOne<GameNight>().WithMany().HasForeignKey(s => s.GameNightId).OnDelete(DeleteBehavior.Cascade);
builder.HasOne<Game>().WithMany().HasForeignKey(s => s.GameId).OnDelete(DeleteBehavior.Restrict);
builder.HasIndex(s => s.GameNightId).HasDatabaseName("IX_Sessions_GameNightId");
builder.HasIndex(s => s.GameId).HasDatabaseName("IX_Sessions_GameId");

// SessionPlayer
builder.ToTable("SessionPlayers");
builder.HasKey(p => new { p.SessionId, p.UserId });
builder.Property(p => p.Score).HasColumnType("decimal(9,2)");
builder.Property(p => p.Placement).IsRequired();
builder.HasOne<Session>().WithMany().HasForeignKey(p => p.SessionId).OnDelete(DeleteBehavior.Cascade);
builder.HasOne<User>().WithMany().HasForeignKey(p => p.UserId).OnDelete(DeleteBehavior.Restrict);
builder.HasIndex(p => p.UserId).HasDatabaseName("IX_SessionPlayers_UserId");
builder.HasIndex(p => new { p.UserId, p.Placement }).HasDatabaseName("IX_SessionPlayers_UserId_Placement");
```

- Check constraint: `CK_Sessions_Duration` `DurationMinutes BETWEEN 1 AND 720 OR DurationMinutes IS NULL`.
- Check constraint: `CK_SessionPlayers_Placement` `Placement >= 1`.

## 8. Test Plan

### Unit Tests
- `[Unit] Session_RequiresAtLeastTwoPlayers`
- `[Unit] Session_DuplicatePlayerUser_Forbidden`
- `[Unit] Session_NonCooperative_HasAtLeastOneFirstPlace`
- `[Unit] Session_Cooperative_RequiresIsCooperativeWinFlag`
- `[Unit] Session_Cooperative_AllPlacementsEqual`

### Integration Tests
- `[Integration] CascadeDelete_Session_RemovesAllPlayersRows`
- `[Integration] Restrict_DeletingUser_WithSessionPlayerRows_Fails`
- `[Integration] Restrict_DeletingGame_WithSessionRows_Fails`
- `[Integration] WinRateQuery_AggregatesAcrossPlayerRowsCorrectly`

## 9. Cross-References

- Logging surface: SDD-MN-CORE-006
- Stats consumers: SDD-MN-CORE-007, SDD-MN-CORE-008
- Parents: SDD-MN-DOM-002 (Game), SDD-MN-DOM-003 (GameNight)
