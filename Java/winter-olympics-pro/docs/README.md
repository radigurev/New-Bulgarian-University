# Winter Olympics — Professional Edition — Documentation

Spec-Driven Documentation (SDD). Specs are organized by concern.

| Category | Folder | Purpose |
|---|---|---|
| Core features | [`core/`](core/) | Externally-visible behavior — what the system does |
| Domain | [`domain/`](domain/) | Domain rules and invariants — what is true in the world |
| Infrastructure | [`infrastructure/`](infrastructure/) | Cross-cutting platform concerns — logging, security, migrations |
| Integration | [`integration/`](integration/) | Boundaries with external systems (today: none — see notes in [SDD-INT-001](integration/SDD-INT-001-future-integrations.md)) |
| Changes | [`changes/`](changes/) | Change-log entries against existing specs |

## Specs

### Core
- [SDD-CORE-001](core/SDD-CORE-001-athlete-management.md) — Athlete management
- [SDD-CORE-002](core/SDD-CORE-002-competition-lifecycle.md) — Competition lifecycle
- [SDD-CORE-003](core/SDD-CORE-003-ski-slalom-results.md) — Ski Slalom results
- [SDD-CORE-004](core/SDD-CORE-004-biathlon-results.md) — Biathlon results
- [SDD-CORE-005](core/SDD-CORE-005-olympics-aggregates.md) — Olympics aggregates
- [SDD-CORE-006](core/SDD-CORE-006-authentication.md) — Authentication and JWT

### Domain
- [SDD-DOM-001](domain/SDD-DOM-001-ranking-strategy.md) — Ranking strategy abstraction

### Infrastructure
- [SDD-INF-001](infrastructure/SDD-INF-001-database-migrations.md) — Liquibase migrations
- [SDD-INF-002](infrastructure/SDD-INF-002-observability.md) — Logging, AOP, Actuator
- [SDD-INF-003](infrastructure/SDD-INF-003-openapi.md) — OpenAPI / Swagger UI

### Integration
- [SDD-INT-001](integration/SDD-INT-001-future-integrations.md) — Future integration surface

See also: [cross-reference-map.md](cross-reference-map.md).

## Drift policy

When implementation diverges from a spec:
1. Decide whether the spec is *authoritative* (drives implementation) or *reflective* (documents what is).
2. For authoritative drift, fix the code to match the spec.
3. For reflective drift, update the spec — bump the Version, append a Change Log row, and add a `changes/<file>.md` entry.
4. Tests must reference spec IDs in their Javadoc when they verify a spec rule.
