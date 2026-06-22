---
id: SDD-MN-CORE-007
title: Personal Statistics Dashboard
status: Planned
version: 1.0
owner: stats
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-006
  - SDD-MN-DOM-004
  - SDD-MN-DOM-002
---

# SDD-MN-CORE-007 — Personal Statistics Dashboard

## 1. Context

Once sessions accumulate, every user gains a personal record. The statistics
dashboard summarises this record in a single page so users can see their playing
habits, strongest games, frequent opponents, and overall record.

Statistics are derived purely from `Session` and `SessionPlayer` rows — there is
no separate aggregate table. Performance is acceptable for the expected volume
(university coursework / single circle of friends; expect < 10k sessions ever).

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/StatisticsController.cs`
- `src/MeepleNight.Web/Views/Statistics/Index.cshtml`
- `src/MeepleNight.Services/StatisticsService.cs`

## 2. Behavior

### 2.1 Authorisation

- **MUST** require authentication.
- **MUST** show only the current user's statistics. (No leaderboard across users
  in v1.0; that would require explicit sharing semantics — out of scope.)

### 2.2 Top-Level Cards

The dashboard **MUST** display the following metrics:

1. **Total sessions played** — count of `SessionPlayer` rows for this user.
2. **Total game nights attended** — count of distinct `GameNightId` reached through
   `Session → GameNight`.
3. **Win rate** — `wins / sessions_with_a_winner_identified` as a percentage.
   Cooperative-game sessions (where `IsCooperativeWin` is set) count toward
   "wins" if the team won.
4. **Most-played game** — the Game with the highest `SessionPlayer` count for this user.
5. **Best-record game** — the Game with the highest win-rate among games played
   ≥ 3 times. If no game qualifies, display "Need more plays".
6. **Frequent opponent** — the user appearing alongside the current user in the
   greatest number of sessions, excluding the current user themselves.

### 2.3 Per-Game Breakdown Table

- **MUST** list every game the user has played, with columns:
  Game, Plays, Wins, Win Rate, Last Played.
- **MUST** be sortable by any column (default: Plays desc).
- **MUST** filter out games with 0 plays.

### 2.4 Activity Timeline

- **MUST** show a 12-month rolling chart of sessions played per month.
- **SHOULD** be a simple bar chart rendered via Chart.js (or any client-side
  charting library accepted by the implementation). No server-side image generation.

### 2.5 Caching

- Statistics are computed on-demand; no caching in v1.0. If queries become slow,
  add an in-memory cache keyed by `userId + lastSessionStampForUser`.

## 3. Validation

No user input on this page beyond optional sort parameters on the breakdown table.

| Query Parameter | Rule |
|---|---|
| `sort` | Optional; ∈ {`game`, `plays`, `wins`, `winrate`, `last`}; default `plays` |
| `dir` | Optional; ∈ {`asc`, `desc`}; default `desc` |

Invalid values fall back to default.

## 4. Errors

| Condition | UI Outcome |
|---|---|
| User has zero sessions | Empty state: "Log your first session to see stats here." with a link to upcoming nights |
| Insufficient data for win-rate calculation (zero finished sessions with winners) | Win-rate card displays "—" |

## 5. Versioning

- New cards or columns are non-breaking additions.
- Removing a card requires a change entry.

## 6. Test Plan

### Unit Tests
- `[Unit] Stats_NoSessions_ReturnsEmptyStateModel`
- `[Unit] Stats_TotalSessions_CountsOnlyCurrentUserPlayerRows`
- `[Unit] Stats_WinRate_ExcludesCooperativeLosesNoWinner_FromDenominator`
- `[Unit] Stats_WinRate_CountsCooperativeWinAsWin`
- `[Unit] Stats_MostPlayedGame_PicksMaxByPlayerCount`
- `[Unit] Stats_BestRecordGame_RequiresAtLeastThreePlays`
- `[Unit] Stats_FrequentOpponent_ExcludesCurrentUserFromCandidates`
- `[Unit] PerGameBreakdown_FiltersOutZeroPlayGames`
- `[Unit] Timeline_ReturnsTwelveBuckets_ZeroPaddedForGapMonths`

### Integration Tests
- `[Integration] DashboardPage_RendersWithRealDatabase_ForSeededUser`

## 7. Cross-References

- Source data: SDD-MN-CORE-006 (Session Logging), SDD-MN-DOM-004 (Session)
- Consumers: SDD-MN-CORE-008 (Play Next uses overlap of recent and best games)
