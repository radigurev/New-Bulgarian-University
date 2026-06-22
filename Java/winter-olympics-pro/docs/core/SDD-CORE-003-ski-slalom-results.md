# SDD-CORE-003 — Ski Slalom Results

| Field | Value |
|---|---|
| ID | SDD-CORE-003 |
| Title | Ski Slalom — two-run competition with qualifier cut |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-002]], [[SDD-DOM-001]] |

## 1. Context

A Ski Slalom competition consists of two runs. Every registered athlete starts
run 1. Only the fastest N athletes (configured via
`app.slalom.qualifiers-for-run-two`, default 30) advance to run 2. The run-2
start order is the reverse of the run-1 ranking — the slowest qualifier starts
first. Final classification is the sum of the two run times.

## 2. Behavior

- **MUST** persist exactly one `SlalomResultEntity` per `(competition, athlete)` pair (enforced by `uk_slalom_comp_athlete`).
- **MUST** record run-1 time only when status is `FINISHED`. Other statuses null the time field.
- **MUST** reject run-2 recording for athletes whose run-1 status is not `FINISHED` or whose run-1 time is null (400).
- **MUST** sort run-2 qualifiers by run-1 time ascending, take the top `qualifiersForRunTwo`, then reverse the order to produce the start list.
- **MUST** delegate ranking computation to [[SDD-DOM-001]] via `SlalomRankingStrategy`. The strategy sums the two run times and excludes any result that is not classified.
- Times are stored as `DECIMAL(8,3)`.

## 3. Validation (`SlalomRunRequest`)

- `athleteId`: `@NotNull`
- `status`: `@NotNull`
- `timeSeconds`: `@DecimalMin("0.001") @Digits(integer=5, fraction=3)` — required when status is `FINISHED`

## 4. Error Rules

| Trigger | HTTP status | Type URI |
|---|---|---|
| Competition is not `SKI_SLALOM` | 400 | `/errors/registration` |
| Run-2 recorded for non-qualifier | 400 | `/errors/registration` |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `SlalomRankingStrategyTest.rank_sortsByTotalTimeAndAssignsMedals_excludingDnf` | Unit | Final ordering, DNF exclusion, medal assignment. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
