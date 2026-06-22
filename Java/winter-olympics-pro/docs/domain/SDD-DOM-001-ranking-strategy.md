# SDD-DOM-001 — Ranking Strategy

| Field | Value |
|---|---|
| ID | SDD-DOM-001 |
| Title | Ranking strategy abstraction |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-003]], [[SDD-CORE-004]], [[SDD-CORE-005]] |

## 1. Context

The system supports two competition types today (Ski Slalom, Biathlon) and is
expected to support more in the future. Each type computes its ranking
differently. The ranking calculation is therefore expressed as a Strategy
pattern with one implementation per `CompetitionType`.

## 2. Behavior

- **MUST** declare `RankingStrategy` with `supports(): CompetitionType` and `rank(competitionId): List<RankingEntryDto>`.
- **MUST** auto-discover all `RankingStrategy` beans via Spring component scanning. `RankingService` holds them in an `EnumMap<CompetitionType, RankingStrategy>` and resolves by `CompetitionEntity.type` at request time.
- **MUST** throw `IllegalStateException` if a competition exists for a type with no registered strategy.
- **MUST** populate `RankingEntryDto.medal` using `Medal.forPosition(position)` — `GOLD`/`SILVER`/`BRONZE` for positions 1–3, `null` otherwise.

## 3. Validation

No external input.

## 4. Error Rules

| Trigger | HTTP status | Notes |
|---|---|---|
| No strategy for type | 500 | `IllegalStateException`. Treated as bug, not user error. |

## 5. Versioning

v1.0. Adding a new competition type requires only adding a new `RankingStrategy` bean and an enum value — no changes to `RankingService`.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `SlalomRankingStrategyTest` | Unit | Slalom-specific rules. |
| `BiathlonRankingStrategyTest` | Unit | Biathlon-specific rules. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
