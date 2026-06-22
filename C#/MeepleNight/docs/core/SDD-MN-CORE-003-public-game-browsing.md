---
id: SDD-MN-CORE-003
title: Public Game Browsing & Search
status: Planned
version: 1.0
owner: catalog
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-002
  - SDD-MN-CORE-002
---

# SDD-MN-CORE-003 — Public Game Browsing & Search

## 1. Context

The game catalogue is the public face of MeepleNight. Anonymous and authenticated
users alike can browse games to decide what to host or to learn about new games.
This is the read-only counterpart to SDD-MN-CORE-002.

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/GamesController.cs`
- `src/MeepleNight.Web/Views/Games/{Index,Details}.cshtml`
- `src/MeepleNight.Services/GameQueryService.cs`

## 2. Behavior

### 2.1 Listing

- **MUST** be reachable at `/Games` without authentication.
- **MUST** display only games where `IsActive = true`.
- **MUST** show, per game card: cover thumbnail, title, players range, duration,
  category badge.
- **MUST** support paging at 12 cards per page (default), with page-size options 12/24/48.
- **MUST** support filtering by:
  - Category (single-select dropdown)
  - Player count (numeric input — only games where `MinPlayers ≤ N ≤ MaxPlayers`)
  - Duration upper bound (≤ N minutes)
- **MUST** support text search on `Title` (case-insensitive, partial match).
- **SHOULD** preserve filter and search parameters in the URL query string so
  results are bookmarkable.
- **SHOULD** sort by Title ascending by default. Other sort options:
  Duration asc/desc, Most-Played-By-Community (count of sessions referencing it).

### 2.2 Detail Page

- **MUST** be reachable at `/Games/Details/{id}` without authentication.
- **MUST** show: full description, cover image, min/max players, duration,
  category, total session count across all users (community count).
- **MUST** return HTTP 404 when the id is unknown OR when the game is soft-deleted
  AND the requester is not an admin.
- **SHOULD** show the user's personal play count when authenticated
  (sessions where this user appeared as a SessionPlayer).

## 3. Validation

| Query Parameter | Rule |
|---|---|
| `page` | Optional, ≥ 1, default 1 |
| `pageSize` | Optional, ∈ {12, 24, 48}, default 12 |
| `category` | Optional, from `GameCategory` enum |
| `players` | Optional, 1–20 |
| `maxDuration` | Optional, 5–720 |
| `search` | Optional, ≤ 100 chars, trimmed |
| `sort` | Optional, ∈ {`title`, `duration_asc`, `duration_desc`, `popularity`}; default `title` |

Invalid query values **MUST** be ignored silently (treated as default), not error
out — this prevents broken bookmarks from killing the page.

## 4. Errors

| Condition | Outcome |
|---|---|
| Unknown game id on Details | HTTP 404, custom Razor 404 view |
| Soft-deleted game accessed by non-admin | HTTP 404 |
| Empty result set | List view shows "No games match your filters." with a "Clear filters" link |

## 5. Versioning

This spec is read-only — additions (new sort options, new filters) are non-breaking
unless they remove existing parameters.

## 6. Test Plan

### Unit Tests
- `[Unit] Index_DefaultRequest_ReturnsActiveGamesPage1Size12_SortedByTitle`
- `[Unit] Index_FilterByCategory_OnlyReturnsMatchingCategory`
- `[Unit] Index_FilterByPlayers_OnlyReturnsGamesWhereRangeIncludesN`
- `[Unit] Index_FilterByMaxDuration_OnlyReturnsGamesWithinLimit`
- `[Unit] Index_TextSearch_MatchesPartialTitleCaseInsensitive`
- `[Unit] Index_SortByPopularity_OrdersBySessionCountDescending`
- `[Unit] Index_InvalidPageSize_FallsBackToDefault`
- `[Unit] Details_UnknownId_Returns404`
- `[Unit] Details_SoftDeletedGame_Returns404ForNonAdmin`
- `[Unit] Details_AuthenticatedUser_ShowsPersonalPlayCount`

### Integration Tests
- `[Integration] AnonymousVisitor_CanReachIndexAndDetails_NoRedirect`
- `[Integration] FilterCombination_ReturnsExpectedRowsFromDatabase`

## 7. Cross-References

- Domain: SDD-MN-DOM-002 (Game)
- Admin counterpart: SDD-MN-CORE-002
- Sessions used for popularity: SDD-MN-DOM-004
