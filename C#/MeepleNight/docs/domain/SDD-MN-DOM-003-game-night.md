---
id: SDD-MN-DOM-003
title: GameNight
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-004
  - SDD-MN-CORE-005
  - SDD-MN-CORE-006
  - SDD-MN-DOM-002
  - SDD-MN-DOM-005
---

# SDD-MN-DOM-003 — GameNight

## 1. Context

A `GameNight` is a scheduled social event hosted by one user. It collects
attendees through `Invitation` (SDD-MN-DOM-005) and accumulates `Session`
records (SDD-MN-DOM-004) once the night happens.

**Key Files (planned):**
- `src/MeepleNight.Domain/Entities/GameNight.cs`
- `src/MeepleNight.Domain/Entities/GameNightCandidate.cs`
- `src/MeepleNight.Domain/Enums/GameNightStatus.cs`
- `src/MeepleNight.Data/Configurations/GameNightConfiguration.cs`

## 2. Properties — GameNight

| Property | Type | Required | Notes |
|---|---|---|---|
| Id | Guid | Yes | PK, `NEWSEQUENTIALID()` default |
| Title | string(100) | Yes | |
| ScheduledForUtc | datetime2(7) | Yes | When the night will start (UTC) |
| Location | string(200) | Yes | Free text |
| Notes | string(1000) | No | |
| ExpectedPlayerCount | int | Yes | 2..20 |
| HostUserId | Guid | Yes | FK to User |
| Status | enum `GameNightStatus` | Yes | `Planned`, `Completed`, `Cancelled` |
| CreatedAtUtc | datetime2(7) | Yes | Default `SYSUTCDATETIME()` |
| UpdatedAtUtc | datetime2(7)? | No | |
| CancelledAtUtc | datetime2(7)? | No | Stamped only on Cancel |

## 3. Properties — GameNightCandidate (join row)

| Property | Type | Required | Notes |
|---|---|---|---|
| GameNightId | Guid | Yes | Composite PK part 1, FK to GameNight |
| GameId | Guid | Yes | Composite PK part 2, FK to Game |
| Position | int | Yes | 1..10, used to preserve display order |

## 4. Relationships

| Relationship | Cardinality | Notes |
|---|---|---|
| GameNight → User (Host) | * → 1 | `HostUserId`; `OnDelete(Restrict)` — host cannot be deleted while owning nights |
| GameNight → Invitation | 1 → * | `Invitation.GameNightId`; `OnDelete(Cascade)` |
| GameNight → Session | 1 → * | `Session.GameNightId`; `OnDelete(Cascade)` |
| GameNight → GameNightCandidate | 1 → * | `OnDelete(Cascade)` |
| GameNightCandidate → Game | * → 1 | `OnDelete(Restrict)` — game cannot be hard-deleted |

## 5. Invariants

- A night **MUST** have at least one Accepted invitation at all times — namely
  the host's auto-acceptance, created by SDD-MN-CORE-004.
- Candidate game count `≤ 10`.
- `ScheduledForUtc > CreatedAtUtc` on creation.
- Status transitions allowed:
  - `Planned → Completed` (via first session logged or manual mark)
  - `Planned → Cancelled` (via host cancel)
  - `Completed → Planned` (via deletion of last remaining session)
  - All other transitions are forbidden.

## 6. EF Core Configuration

```
builder.ToTable("GameNights");
builder.HasKey(n => n.Id);
builder.Property(n => n.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
builder.Property(n => n.Title).HasMaxLength(100).IsRequired();
builder.Property(n => n.Location).HasMaxLength(200).IsRequired();
builder.Property(n => n.Notes).HasMaxLength(1000);
builder.Property(n => n.Status).HasConversion<string>().HasMaxLength(20);
builder.Property(n => n.ScheduledForUtc).HasColumnType("datetime2(7)");
builder.Property(n => n.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
builder.Property(n => n.UpdatedAtUtc).HasColumnType("datetime2(7)");
builder.Property(n => n.CancelledAtUtc).HasColumnType("datetime2(7)");

builder.HasIndex(n => n.HostUserId).HasDatabaseName("IX_GameNights_HostUserId");
builder.HasIndex(n => n.ScheduledForUtc).HasDatabaseName("IX_GameNights_ScheduledForUtc");
builder.HasIndex(n => n.Status).HasDatabaseName("IX_GameNights_Status");

builder.HasOne<User>().WithMany().HasForeignKey(n => n.HostUserId).OnDelete(DeleteBehavior.Restrict);
```

For `GameNightCandidate`:

```
builder.ToTable("GameNightCandidates");
builder.HasKey(c => new { c.GameNightId, c.GameId });
builder.HasOne<GameNight>().WithMany().HasForeignKey(c => c.GameNightId).OnDelete(DeleteBehavior.Cascade);
builder.HasOne<Game>().WithMany().HasForeignKey(c => c.GameId).OnDelete(DeleteBehavior.Restrict);
builder.Property(c => c.Position).IsRequired();
```

- Check constraint: `CK_GameNights_ExpectedPlayers` `BETWEEN 2 AND 20`.
- Check constraint: `CK_GameNightCandidates_Position` `BETWEEN 1 AND 10`.

## 7. Test Plan

### Unit Tests
- `[Unit] GameNight_StatusTransitionPlannedToCompleted_Allowed`
- `[Unit] GameNight_StatusTransitionCompletedToCancelled_Forbidden`
- `[Unit] GameNight_ExpectedPlayerCountOutOfRange_FailsValidation`
- `[Unit] GameNightCandidate_PositionOutOfRange_FailsValidation`

### Integration Tests
- `[Integration] CascadeDelete_GameNight_RemovesInvitationsAndSessions`
- `[Integration] Restrict_DeletingHost_WhenOwningGameNight_Fails`

## 8. Cross-References

- Feature flows: SDD-MN-CORE-004, 005, 006
- Children: SDD-MN-DOM-005 (Invitation), SDD-MN-DOM-004 (Session)
