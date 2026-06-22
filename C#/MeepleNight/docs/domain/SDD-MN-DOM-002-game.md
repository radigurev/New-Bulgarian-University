---
id: SDD-MN-DOM-002
title: Game
status: Planned
version: 1.0
owner: catalog
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-002
  - SDD-MN-CORE-003
  - SDD-MN-CORE-006
---

# SDD-MN-DOM-002 — Game

## 1. Context

A `Game` represents a board game in the shared MeepleNight catalogue. Games are
referenced by candidate game lists on game nights and by every session.

**Key Files (planned):**
- `src/MeepleNight.Domain/Entities/Game.cs`
- `src/MeepleNight.Domain/Enums/GameCategory.cs`
- `src/MeepleNight.Data/Configurations/GameConfiguration.cs`

## 2. Properties

| Property | Type | Required | Notes |
|---|---|---|---|
| Id | Guid | Yes | PK, default `NEWSEQUENTIALID()` |
| Title | string(100) | Yes | Unique among `IsActive = true` rows (case-insensitive) |
| Description | string(2000) | No | Plain text or limited markdown |
| MinPlayers | int | Yes | ≥ 1 |
| MaxPlayers | int | Yes | ≥ MinPlayers, ≤ 20 |
| AverageDurationMinutes | int | Yes | 5..720 |
| Category | enum `GameCategory` | Yes | Strategy, Family, Party, Cooperative, RPG, Abstract, Other |
| CoverImagePath | string(260) | No | Relative path under `wwwroot/uploads/games/` |
| IsActive | bool | Yes | Default `true`; soft-delete flag |
| CreatedAtUtc | datetime2(7) | Yes | Default `SYSUTCDATETIME()` |
| CreatedByUserId | Guid? | No | FK to User; null for seed data |
| UpdatedAtUtc | datetime2(7)? | No | Stamped on every save |
| UpdatedByUserId | Guid? | No | FK to User |

## 3. Relationships

| Relationship | Cardinality | Notes |
|---|---|---|
| Game → Session | 1 → * | `Session.GameId` FK |
| Game → GameNightCandidate | 1 → * | join table linking game nights to candidate games |

## 4. Invariants

- `MaxPlayers >= MinPlayers`
- `MinPlayers >= 1`
- `MaxPlayers <= 20`
- `AverageDurationMinutes >= 5`
- `AverageDurationMinutes <= 720`
- `Title` uniqueness: only enforced among `IsActive = true` rows. A soft-deleted
  game named "Catan" does not block creating a new active "Catan" — but admins
  are encouraged to restore the existing row instead.
- Hard delete forbidden — referential integrity from sessions must be preserved.

## 5. EF Core Configuration

```
builder.ToTable("Games");
builder.HasKey(g => g.Id);
builder.Property(g => g.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
builder.Property(g => g.Title).HasMaxLength(100).IsRequired();
builder.Property(g => g.Description).HasMaxLength(2000);
builder.Property(g => g.Category).HasConversion<string>().HasMaxLength(20);
builder.Property(g => g.CoverImagePath).HasMaxLength(260);
builder.Property(g => g.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
builder.Property(g => g.UpdatedAtUtc).HasColumnType("datetime2(7)");
builder.HasIndex(g => g.Title).HasDatabaseName("IX_Games_Title");
builder.HasIndex(g => new { g.IsActive, g.Category }).HasDatabaseName("IX_Games_IsActive_Category");
builder.HasOne<User>().WithMany().HasForeignKey(g => g.CreatedByUserId).OnDelete(DeleteBehavior.SetNull);
```

- Constraint: `CK_Games_PlayerRange` enforces `MaxPlayers >= MinPlayers`.
- Constraint: `CK_Games_Duration` enforces `AverageDurationMinutes BETWEEN 5 AND 720`.

## 6. Seed Data

The initial migration (per SDD-MN-INF-003) **MUST** seed a small starter set of
games (e.g. Catan, Wingspan, Codenames, Pandemic, Dixit) so the empty-state UI
isn't jarring on first run. Seed data has `CreatedByUserId = null`.

## 7. Test Plan

### Unit Tests
- `[Unit] Game_MaxPlayersLessThanMin_FailsValidation`
- `[Unit] Game_DurationOutOfRange_FailsValidation`
- `[Unit] Game_DefaultIsActiveTrue_OnConstruction`

### Integration Tests
- `[Integration] Game_DuplicateActiveTitle_RejectedByUniqueFilteredIndex`
- `[Integration] Game_SoftDeletedSameTitle_ActiveCreationAllowed`
- `[Integration] Game_CategoryEnum_PersistsAsString`

## 8. Cross-References

- Admin: SDD-MN-CORE-002
- Public browse: SDD-MN-CORE-003
- Session: SDD-MN-CORE-006, SDD-MN-DOM-004
