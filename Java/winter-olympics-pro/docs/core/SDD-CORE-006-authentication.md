# SDD-CORE-006 — Authentication and Authorization

| Field | Value |
|---|---|
| ID | SDD-CORE-006 |
| Title | Authentication (form login + JWT) and authorization |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-001]], [[SDD-CORE-002]] |

## 1. Context

Three actors interact with the system:
- **Athlete** — registers themselves, edits own profile, signs up for competitions.
- **Admin** — manages competitions and records results.
- **Anonymous public** — reads rankings, medals, and aggregate stats.

Authentication uses BCrypt-hashed passwords stored in `user_accounts`. The web
UI uses form-login sessions; the REST API uses stateless JWT bearer tokens
issued by `POST /api/v1/auth/login`.

## 2. Behavior

- **MUST** seed a default `admin` account with role `ROLE_ADMIN` on first startup via Liquibase changelog `v1.0.1-seed-admin-role.yaml`.
- **MUST** allow self-registration via `POST /api/v1/auth/register` and `GET/POST /register` (form route). Self-registration grants `ROLE_ATHLETE` and creates a linked `AthleteEntity`.
- **MUST** issue a JWT at `POST /api/v1/auth/login`. The token carries: subject = username, `uid`, `roles` claim, issuer = `app.security.jwt.issuer`, expiry = `app.security.jwt.ttl-minutes`.
- **MUST** parse and accept JWT bearer tokens via `JwtAuthenticationFilter` for all `/api/**` requests.
- **MUST** secure `/api/v1/admin/**` and all `POST/PUT/DELETE` mutation endpoints behind `ROLE_ADMIN`.
- **MUST** allow `ROLE_ATHLETE` to self-register for a competition.
- **MUST** allow anonymous read access to `/api/v1/public/**` and to `GET /api/v1/athletes` and `GET /api/v1/competitions`.
- **MUST** disable CSRF on the API filter chain (stateless).
- **MUST** keep CSRF enabled on the web filter chain (session-based).

## 3. Validation

- `RegisterRequest.username`: `@NotBlank @Size(min=3, max=60)` and unique.
- `RegisterRequest.password`: `@NotBlank @Size(min=6, max=100)`.
- `RegisterRequest.athlete`: cascaded validation per [[SDD-CORE-001]].

## 4. Error Rules

| Trigger | HTTP status | Notes |
|---|---|---|
| Bad credentials | 401 | Returned via Spring Security default; mapped to ProblemDetail by [[SDD-INF-002]] handler. |
| Token expired/invalid | 401 | `JwtAuthenticationFilter` silently clears the security context; downstream returns 401. |
| Missing required authority | 403 | RFC 7807 ProblemDetail `/errors/forbidden`. |
| Username already taken | 400 | `RegistrationException` → `/errors/registration`. |

## 5. Versioning

v1.0. Migration to OAuth2/Keycloak would be a v2.0 breaking change.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `AthleteControllerIT.create_returnsCreated_forAdmin` | Integration | `ROLE_ADMIN` admits mutation |
| `AthleteControllerIT.create_returnsForbidden_forAthlete` | Integration | `ROLE_ATHLETE` rejected for admin endpoints |
| `AthleteControllerIT.list_isPubliclyAccessible` | Integration | Anonymous read |
| (Manual) | API smoke | `POST /api/v1/auth/login` returns a token; reusing in `Authorization: Bearer ...` admits subsequent requests. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
