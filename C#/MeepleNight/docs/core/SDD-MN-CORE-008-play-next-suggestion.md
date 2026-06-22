---
id: SDD-MN-CORE-008
title: "Play Next" Suggestion Engine
status: Planned
version: 1.0
owner: stats
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-006
  - SDD-MN-CORE-007
  - SDD-MN-DOM-004
  - SDD-MN-DOM-002
---

# SDD-MN-CORE-008 — "Play Next" Suggestion Engine

## 1. Context

When a host is planning a new game night, choosing what to play is half the fun
and half the friction. The "Play Next" engine recommends 5 games based on the
host's recent activity — favouring games the host enjoys but hasn't played in a
while, and ensuring suggestions match the expected player count.

This is a deliberately simple rule-based engine — no ML, no embeddings — chosen
to keep scope realistic for a coursework project while still demonstrating
useful behaviour beyond plain CRUD.

**Key Files (planned):**
- `src/MeepleNight.Services/PlayNextSuggestionService.cs`
- `src/MeepleNight.Web/Controllers/GameNightsController.cs` (consumes service in Create)

## 2. Behavior

### 2.1 Trigger

- **MUST** be invoked automatically on the Game Night Create page, as a
  side-panel widget titled "Suggestions".
- **MUST** also be reachable as an API endpoint at
  `GET /api/playnext?players={n}` returning a JSON list — used by the candidate-game
  picker on the Create page when the user changes `ExpectedPlayerCount`.

### 2.2 Algorithm

For the authenticated user, with input `expectedPlayerCount` (defaulting to 4 when not provided):

1. **Eligibility filter** — only consider Games where:
   - `IsActive = true`
   - `MinPlayers ≤ expectedPlayerCount ≤ MaxPlayers`
2. **Score each candidate game** as:
   `score = personalAffinity * 0.6 + cooldownBonus * 0.4`
   where:
   - `personalAffinity` = the user's win rate on this game (0..1) when ≥ 3 plays;
     otherwise `0.5` (neutral prior). Cooperative wins count.
   - `cooldownBonus` = `min(daysSinceLastPlay, 90) / 90` (0..1). If the user has
     never played the game, `cooldownBonus = 1.0`.
3. **Diversity** — when two candidates have identical category and score within
   `0.05`, keep the higher-scored one and demote the other. Avoids returning
   five Strategy games in a row.
4. **Tie-break** — by `Title` ascending.
5. **Return** — top 5 games, with `score`, `category`, `playerRange`, and a one-line
   reason ("Haven't played in X days", "Strong record (W%)", "New to you").

### 2.3 Empty Result Handling

- **MUST** still return up to 5 games when the user has zero sessions, falling
  back to a global popularity list (most sessions across all users) restricted by
  the player-count filter.

## 3. Validation

| Input | Rule |
|---|---|
| `players` | Optional, 1–20, default 4. Out-of-range values fall back to default. |

## 4. Errors

| Condition | Outcome |
|---|---|
| No active games match the player filter | Return empty list with a `meta.reason = "no_eligible_games"` field |
| Service exception | Return empty list and log the exception (per SDD-MN-INF-001). The Create page renders an inline notice: "Suggestions temporarily unavailable." Failure here **MUST NOT** block the host from creating a night. |

## 5. Versioning

- The scoring formula is **internal** and may change without a breaking-change
  notice. Tests should assert *ordering by category of game* (e.g. recently
  cold-shouldered favourites rank above never-played) rather than exact numeric
  scores, so they don't break on tuning.
- Adding new factors (e.g. "guests like this game too") is a non-breaking addition.

## 6. Test Plan

### Unit Tests
- `[Unit] Suggest_FiltersByPlayerCount_OnlyEligibleGamesReturned`
- `[Unit] Suggest_FavoursHighWinRateFavourites_OverNeutralPriors`
- `[Unit] Suggest_AppliesCooldown_ColdGamesScoreHigherThanRecentlyPlayed`
- `[Unit] Suggest_NeverPlayedGame_GetsCooldownBonusOne`
- `[Unit] Suggest_LessThanThreePlays_UsesNeutralPrior`
- `[Unit] Suggest_DiversityRule_DemotesSecondSameCategoryNearTie`
- `[Unit] Suggest_TieBreak_OrdersByTitleAscending`
- `[Unit] Suggest_NewUserNoSessions_FallsBackToGlobalPopularity`
- `[Unit] Suggest_NoEligibleGames_ReturnsEmptyWithReason`
- `[Unit] Suggest_ServiceException_DoesNotPropagate_ReturnsEmpty`

### Integration Tests
- `[Integration] PlayNextEndpoint_ReturnsJsonForAuthenticatedUser`
- `[Integration] PlayNextEndpoint_AnonymousUser_Returns401`

## 7. Cross-References

- Source data: SDD-MN-DOM-004 (Session), SDD-MN-DOM-002 (Game)
- Companion feature: SDD-MN-CORE-007 (Statistics — same source)
- Consumed by: SDD-MN-CORE-004 (Game Night Create page UI)
