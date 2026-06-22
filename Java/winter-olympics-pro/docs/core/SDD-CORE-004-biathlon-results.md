# SDD-CORE-004 — Biathlon Results

| Field | Value |
|---|---|
| ID | SDD-CORE-004 |
| Title | Biathlon — single base time plus shooting penalties |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-002]], [[SDD-DOM-001]] |

## 1. Context

A Biathlon competition stores one base ski time per athlete plus a number of
shooting misses. Each miss adds a fixed penalty (configured via
`app.biathlon.penalty-seconds-per-miss`, default 60 — one minute). Final time
is `baseTimeSeconds + (misses × penaltySecondsPerMiss)`.

## 2. Behavior

- **MUST** persist exactly one `BiathlonResultEntity` per `(competition, athlete)` pair (enforced by `uk_biathlon_comp_athlete`).
- **MUST** clear `baseTimeSeconds` when status is not `FINISHED`.
- **MUST** delegate ranking computation to [[SDD-DOM-001]] via `BiathlonRankingStrategy`. The strategy excludes any result that is not classified.
- Times are stored as `DECIMAL(8,3)`.

## 3. Validation (`BiathlonResultRequest`)

- `athleteId`: `@NotNull`
- `status`: `@NotNull`
- `baseTimeSeconds`: `@DecimalMin("0.001") @Digits(integer=5, fraction=3)` — required when status is `FINISHED`
- `misses`: `@Min(0)`

## 4. Error Rules

| Trigger | HTTP status | Type URI |
|---|---|---|
| Competition is not `BIATHLON` | 400 | `/errors/registration` |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `BiathlonRankingStrategyTest.rank_appliesPenaltiesExcludesDnfAndAssignsMedals` | Unit | Penalty arithmetic, DNF exclusion, ordering, medal assignment. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
