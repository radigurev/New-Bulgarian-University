# SDD-CORE-005 — Olympics Aggregates

| Field | Value |
|---|---|
| ID | SDD-CORE-005 |
| Title | Public aggregates — medals, average age, youngest/oldest medalist |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-003]], [[SDD-CORE-004]] |

## 1. Context

Aggregates expose a public summary of the games to the press and to the audience
without requiring authentication.

## 2. Behavior

- **MUST** expose `GET /api/v1/public/stats` and view routes `/stats`, `/medals` without authentication.
- **MUST** include only competitions with `finished=true` when computing the medal table.
- **MUST** compute the medal table by summing per athlete country across finished competitions and ordering by `gold DESC, silver DESC, bronze DESC`.
- **MUST** compute average age as `Period.between(dateOfBirth, today).years` averaged over *all* athletes (registered or not).
- **MUST** report `youngestMedalist` (latest `dateOfBirth`) and `oldestMedalist` (earliest `dateOfBirth`) across medalists in finished competitions. Both `null` when no medalists exist yet.
- **MUST** include scalar counts `totalAthletes` and `totalCompetitions`.

## 3. Validation

No user input.

## 4. Error Rules

| Trigger | HTTP status | Detail |
|---|---|---|
| Generic failure | 500 | RFC 7807 ProblemDetail |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| (covered indirectly) | UI walkthrough | `/medals` and `/stats` render correctly with seeded data; ranking tests cover the medal computation. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
