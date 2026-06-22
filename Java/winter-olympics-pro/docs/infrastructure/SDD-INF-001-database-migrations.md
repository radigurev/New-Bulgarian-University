# SDD-INF-001 — Database Migrations

| Field | Value |
|---|---|
| ID | SDD-INF-001 |
| Title | Liquibase-managed schema migrations |
| Status | Implemented |
| Version | 1.0 |
| Last updated | 2026-05-18 |
| Related specs | [[SDD-CORE-001]], [[SDD-CORE-002]], [[SDD-CORE-006]] |

## 1. Context

The database schema is owned by Liquibase, not by Hibernate. JPA runs with
`spring.jpa.hibernate.ddl-auto=validate` against the schema produced by
Liquibase changelogs.

## 2. Behavior

- **MUST** declare the master changelog at `classpath:db/changelog/db.changelog-master.yaml`.
- **MUST** version each migration as `v<Major>.<Minor>.<Patch>-<short-description>.yaml` under `db/changelog/changes/`.
- **MUST** assign each `changeSet` an `id` of the form `<version>-<sequence>-<short-name>` and set `author: olympics`.
- **MUST NOT** modify a previously-applied changeset. New changes go into new changesets.
- **MUST** use `preConditions` for seed data and reference-data inserts so they are idempotent (e.g., the admin seed checks `COUNT(*) = 0` before inserting).
- **MUST** name primary keys `pk_<table>`, foreign keys `fk_<child>_<parent>`, unique constraints `uk_<table>_<col1>_<col2>`, and indexes `idx_<table>_<col>`.

## 3. Validation

The Liquibase changelog is its own contract. CI must run `validate` against
both the dev (H2) and prod (MySQL) profiles before merging.

## 4. Error Rules

| Trigger | Behavior |
|---|---|
| Liquibase fails to apply on startup | Application fails fast — Spring Boot context refresh aborts. |
| Schema does not match entity expectations | Hibernate `validate` throws `SchemaManagementException` on startup. |

## 5. Versioning

v1.0.0 ships the initial schema. v1.0.1 seeds the default admin account.

## 6. Test Plan

| Test | Type | Verifies |
|---|---|---|
| `WinterOlympicsProApplicationTests.contextLoads` | `@SpringBootTest` | Application boots, validating Liquibase and entity wiring. |

## 7. Change Log

| Version | Date | Change |
|---|---|---|
| 1.0 | 2026-05-18 | Initial spec. |
