# SDD-INT-001 — Future Integration Surface

| Field | Value |
|---|---|
| ID | SDD-INT-001 |
| Title | Planned external integrations (today: none) |
| Status | Planned |
| Version | 0.1 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-006]] |

## 1. Context

The professional edition is a monolith and ships with no external integrations
on day one. This spec exists to keep the integration boundary visible so future
work can be slotted in without architectural surprise.

## 2. Planned integrations (NOT YET IMPLEMENTED)

- **Identity provider** — replace the in-house user table with Keycloak or
  another OIDC provider. The plumbing already exists at the JWT layer
  ([[SDD-CORE-006]]) so this becomes "swap `JwtService.issue/parse` for OIDC
  introspection / JWKS validation."
- **Live timing feed** — receive chip-timing events from a slope-side system
  over WebSocket or message queue. Expected payload: `{competitionId, athleteId, run, status, timeSeconds}`. Mapping target: `SlalomService.recordRunOne` / `recordRunTwo` / `BiathlonService.recordResult`.
- **Public results syndication** — publish results to an external "Olympics
  results" feed via outbound HTTP. The right insertion point is an event
  listener on competition `finished=true` transitions.

## 3. Out of scope (explicitly)

- Microservices decomposition. The current monolith handles the projected
  volume (one Olympics → at most 200 athletes × 20 competitions = 4,000
  classified results).
- Real-time data push to spectators. The aggregated stats are read-on-demand.

## 4. Error Rules

n/a — no implementation yet.

## 5. Versioning

v0.1 — placeholder; each integration will get its own SDD-INT-NNN when work starts.

## 6. Test Plan

n/a.

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 0.1 | 2026-05-18 | Initial placeholder spec. |
