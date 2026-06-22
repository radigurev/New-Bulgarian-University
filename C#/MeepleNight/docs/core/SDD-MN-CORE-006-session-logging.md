---
id: SDD-MN-CORE-006
title: Game Session Logging
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-004
  - SDD-MN-DOM-003
  - SDD-MN-DOM-002
  - SDD-MN-CORE-007
---

# SDD-MN-CORE-006 — Game Session Logging

## 1. Context

After a game night happens, the host records what was actually played. Each
**Session** captures a single game played during a night, with its participants,
final scores, and who won. Sessions are the source of truth for personal
statistics (SDD-MN-CORE-007) and the play-next suggestion engine (SDD-MN-CORE-008).

Logging is **host-only** because verifying score memory across multiple players
in a UI is unreliable — the host writes one canonical record. (A future spec
could add per-player confirmation; out of scope here.)

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/SessionsController.cs`
- `src/MeepleNight.Web/Views/Sessions/{Create,Edit,_PlayerScoreRow}.cshtml`
- `src/MeepleNight.Services/SessionService.cs`
- `src/MeepleNight.Services/Validators/SessionValidator.cs`

## 2. Behavior

### 2.1 Permission

- **MUST** allow only the host of the parent game night to create, edit, or
  delete sessions on that night.
- **MUST** allow logging only when the night status is `Planned` or `Completed`
  and `ScheduledForUtc <= now + 24 hours` (i.e. the night has started or is happening).
  Logging far in advance is forbidden; logging well after the fact is allowed.

### 2.2 Create

- **MUST** capture: GameId (one of the candidate or any active game), StartedAtUtc
  (defaulted to now, editable), DurationMinutes (optional), WinnerNote (optional
  free text — e.g. "Tied between A and B"), and a list of `SessionPlayer` rows.
- **MUST** require ≥ 2 player rows.
- **MUST** require each player row to reference a unique user who is either:
  (a) an Accepted invitee of the parent night, OR (b) the host themselves.
- **MUST** capture per-player `Score` (decimal, optional — some games don't have
  scores) and a `Placement` (1 = winner, 2 = runner-up, …; ties allowed via
  shared placement values).
- **MUST** identify exactly one **winner** by either:
  - Lowest `Placement` value (when scores aren't used), OR
  - Highest `Score` (when scores are used and the game's category is Strategy/Family/Abstract), OR
  - Lowest `Score` (when scores are used and the game's category is Cooperative — the team wins or loses; in the cooperative case the user can mark `IsCooperativeWin = true/false` and no individual winner is identified).
- **MAY** support multiple winners (ties): all rows with `Placement = 1` are winners.
- **MUST** transition the parent night's status from `Planned` to `Completed` on
  the first session created.

### 2.3 Edit

- **MUST** allow the host to edit any field of a session up to 7 days after
  the parent night's `ScheduledForUtc`. After 7 days, sessions become read-only
  to keep statistics stable.

### 2.4 Delete

- **MUST** allow the host to delete a session within the same 7-day editable window.
- **MUST** be a **hard delete** (rows go away). Statistics will recompute on next view.
- **MUST** revert the parent night's status to `Planned` when the last remaining
  session is deleted.

### 2.5 Display

- **MUST** show, on the night's Details page, a chronological list of sessions
  with: game title, players, scores, winner highlight, duration.

## 3. Validation

| Field | Rule |
|---|---|
| GameId | Required, refers to an active Game OR a soft-deleted Game already linked to existing sessions of this night |
| StartedAtUtc | Required, ≥ parent night `ScheduledForUtc - 12h`, ≤ now |
| DurationMinutes | Optional, 1–720 |
| Players | Required, ≥ 2, all distinct, all are valid attendees or the host |
| Player.Score | Optional decimal, –9999 to 9999 |
| Player.Placement | Required, ≥ 1 |
| WinnerNote | Optional, ≤ 200 chars |
| IsCooperativeWin | Required when Game.Category = Cooperative; otherwise null |

## 4. Errors

| Condition | UI Outcome |
|---|---|
| Player not in attendee list | Field error: "{Name} did not RSVP to this night." |
| Duplicate player rows | Field error: "Each player can appear only once per session." |
| Edit/Delete after 7 days | HTTP 400: "Sessions become read-only 7 days after the night." |
| Multiple Placement = 1 across non-tied scores | Server-side validation error |

## 5. Versioning

- Adding scoring categories (e.g. Auction-style cumulative scoring) requires a
  new `ScoringMode` enum and a migration.
- Changing the 7-day edit window length is breaking and requires a change entry.

## 6. Test Plan

### Unit Tests
- `[Unit] Create_AsHost_PersistsSessionAndCompletesNight`
- `[Unit] Create_AsAccepted Invitee_ReturnsForbid`
- `[Unit] Create_WithSinglePlayer_ReturnsValidationError`
- `[Unit] Create_WithDuplicatePlayers_ReturnsValidationError`
- `[Unit] Create_WithPlayerNotAttending_ReturnsValidationError`
- `[Unit] Create_StrategyGameWithScores_HighestScoreIsWinner`
- `[Unit] Create_CooperativeGame_RequiresIsCooperativeWinFlag`
- `[Unit] Create_CooperativeGame_NoIndividualWinnerWhenLost`
- `[Unit] Create_TiedFirstPlace_AllowsMultipleWinners`
- `[Unit] Edit_WithinSevenDays_AllowsUpdates`
- `[Unit] Edit_AfterSevenDays_ReturnsBadRequest`
- `[Unit] Delete_LastSession_RevertsNightToPlanned`

### Integration Tests
- `[Integration] CreateFirstSession_TransitionsNightToCompleted_AndPersistsPlayers`
- `[Integration] DeleteAllSessions_RevertsNightStatus`

## 7. Cross-References

- Domain: SDD-MN-DOM-004 (Session, SessionPlayer)
- Predecessors: SDD-MN-CORE-004 (Game Night), SDD-MN-CORE-005 (RSVP — accepted attendees)
- Consumers: SDD-MN-CORE-007 (Statistics), SDD-MN-CORE-008 (Play Next)
