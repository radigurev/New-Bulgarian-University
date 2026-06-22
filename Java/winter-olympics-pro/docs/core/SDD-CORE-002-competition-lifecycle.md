# SDD-CORE-002 — Competition Lifecycle

| Field | Value |
|---|---|
| ID | SDD-CORE-002 |
| Title | Competition lifecycle and registration |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-001]], [[SDD-CORE-003]], [[SDD-CORE-004]], [[SDD-CORE-005]] |

## 1. Context

A competition is a scheduled event for one competition type
(`SKI_SLALOM` or `BIATHLON`) and one gender. Each enforces a minimum age
and runs through three states: *upcoming → results being recorded → finished*.
Athletes must be registered before times can be recorded for them.

## 2. Behavior

- **MUST** expose CRUD over REST at `/api/v1/competitions` (admin-only mutation).
- **MUST** expose `GET /api/v1/competitions/{id}/ranking` returning a type-appropriate ranking via [[SDD-DOM-001]].
- **MUST** allow `POST /api/v1/competitions/{id}/finish` (admin) to transition a competition to `finished`. There is no "un-finish" action.
- **MUST** allow `POST /api/v1/competitions/{competitionId}/registrations/{athleteId}` for `ROLE_ADMIN` or `ROLE_ATHLETE`.
- **MUST** reject registration when:
  - the competition is finished;
  - the athlete's gender does not match the competition's;
  - the athlete is younger than `minAge` on `heldOn`;
  - the (competition, athlete) pair is already registered (DB unique constraint `uk_registration_comp_athlete`).
- **MUST** version every row via JPA optimistic locking (`@Version`) — see [[SDD-INF-001]].

## 3. Validation (`CreateCompetitionRequest`)

- `name`: `@NotBlank @Size(min=3, max=120)`
- `type`: `@NotNull`
- `gender`: `@NotNull`
- `minAge`: `@Min(15)`
- `heldOn`: `@NotNull`

## 4. Error Rules

| Trigger | HTTP status | Type URI |
|---|---|---|
| Competition not found | 404 | `/errors/not-found` |
| Registration rule violation | 400 | `/errors/registration` |
| Validation failure | 400 | `/errors/validation` |
| Missing role | 403 | `/errors/forbidden` |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `CompetitionServiceTest.register_rejectsWhenCompetitionFinished` | Unit | Finished competition rejects new registrations. |
| `CompetitionServiceTest.register_rejectsWhenGenderMismatch` | Unit | Gender match enforced. |
| `CompetitionServiceTest.register_rejectsWhenUnderMinAge` | Unit | Age check uses `heldOn` as the reference date. |
| `CompetitionServiceTest.register_rejectsDuplicate` | Unit | Duplicate registration rejected before DB constraint fires. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
