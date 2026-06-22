# SDD-CORE-001 — Athlete Management

| Field | Value |
|---|---|
| ID | SDD-CORE-001 |
| Title | Athlete management |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-002]], [[SDD-CORE-006]] |

## 1. Context

Athletes are the primary subjects of the system. Each athlete is uniquely
identifiable and carries the demographic data needed to enforce eligibility
rules (gender split, minimum age).

## 2. Behavior

- **MUST** expose CRUD over REST at `/api/v1/athletes` (v1 versioned).
- **MUST** allow anonymous reads (`GET /api/v1/athletes`, `GET /api/v1/athletes/{id}`).
- **MUST** require `ROLE_ADMIN` for POST/PUT/DELETE.
- **MUST** support filtering by `country` (case-insensitive equals) and `lastName` (case-insensitive contains) via query parameters.
- **MUST** persist athletes via JPA against the schema described by Liquibase changelog v1.0.0.

## 3. Validation (`CreateAthleteRequest`)

- `firstName`: `@NotBlank @Size(min=2, max=100)`
- `lastName`: `@NotBlank @Size(min=2, max=100)`
- `country`: `@NotBlank @Size(min=2, max=60)`
- `gender`: `@NotNull`
- `dateOfBirth`: `@NotNull @Past`

Validation failures return RFC 7807 ProblemDetail with HTTP 400.

## 4. Error Rules

| Trigger | HTTP status | Detail |
|---|---|---|
| Athlete id not found | 404 | `Athlete with id {id} was not found` |
| Validation failure | 400 | Concatenated `field: message` list |
| Caller lacks `ROLE_ADMIN` on mutation | 403 | "Access denied" |

## 5. Versioning

v1.0.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `AthleteControllerIT.list_isPubliclyAccessible` | Integration | Anonymous read |
| `AthleteControllerIT.create_returnsCreated_forAdmin` | Integration | Admin can create |
| `AthleteControllerIT.create_returnsForbidden_forAthlete` | Integration | Non-admin gets 403 |
| `AthleteRepositoryIT.findAllByCountryIgnoreCase_matchesByCountry` | `@DataJpaTest` | Derived-query case insensitivity |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
