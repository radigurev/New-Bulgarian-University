# ForumGuard Documentation (SDD)

> Project: **ForumGuard** — Forum with automatic moderation of rude comments (CSCB634 practice project)
> Documentation model: **Spec-Driven Development (SDD)**
> Status: **GREENFIELD** — specs are AUTHORITATIVE (they define what WILL be built).
> Last updated: 2026-06-21

This folder holds the authoritative specifications for ForumGuard. No production code exists yet;
the specs define **what the system must do**, and implementation (Phase 2) must conform to them.

---

## Two-Tier Documentation Model

ForumGuard documentation has two tiers:

1. **System Specs** (`SDD-FORUM-NNN`) — long-lived, authoritative descriptions of a feature,
   domain concept, infrastructure concern, or integration. One bounded concept per file. These are
   the source of truth that implementation and tests are validated against. Stored in the
   category folders below.

2. **Change Specs** (`CHG-<TYPE>-NNN`) — scoped records of a single modification to one or more
   system specs. A change spec amends a system spec; it never replaces it. Stored in `changes/`.
   Start every change spec from `changes/_TEMPLATE.md`.

---

## Conventions

### Spec ID format

- System specs: `SDD-FORUM-NNN` (domain segment is always `FORUM`).
- File name: `SDD-FORUM-NNN-<short-description>.md`, placed in the matching category folder.
- Change specs: `CHG-<TYPE>-NNN-<short-description>.md` in `changes/`.
- IDs are never reused for unrelated behavior. Material behavior changes increment the spec's
  version notes (v1, v2, …) — the ID stays stable.

### Required sections (every system spec, in order)

1. **Header** — Title, Spec ID, Status, Last updated, Owner.
2. **Context & Scope** — covered / excluded; related specs cross-referenced by ID.
3. **Behavior** — happy path first + at least 2 edge cases; RFC-2119 MUST/SHOULD/MAY; every MUST/SHOULD testable.
4. **Validation Rules** — field-level, cross-field, state-based.
5. **Error Rules** — trigger; type (validation/conflict/notfound/authorization); domain mapping (exception or `Result<T>`); Razor user-facing outcome.
6. **Versioning Notes** — at least "v1 — Initial specification".
7. **Test Plan** — explicit test names + `[Unit]` / `[Integration]` tags.

### Test naming

- Convention: `MethodName_Scenario_ExpectedResult` (e.g., `SubmitComment_CleanText_PublishesImmediately`).
- Tag each test `[Unit]` or `[Integration]`.
- Business tests reference their spec via `[Category("SDD-FORUM-NNN")]`.
- Infrastructure-only tests may omit a spec ID but must use `[Category("Infrastructure")]`.

---

## Directory Structure

| Path | Holds |
|---|---|
| `docs/README.md` | This file — documentation index and conventions. |
| `docs/cross-reference-map.md` | Bidirectional map: spec ID ↔ planned `src/` files. |
| `docs/core/` | Core business features, workflows, use cases. |
| `docs/domain/` | Domain concepts, entities, value objects, lifecycle/business rules. |
| `docs/infrastructure/` | Cross-cutting concerns: ML analysis, data access, configuration/options, ML model training & packaging. |
| `docs/integration/` | External systems and tooling. _(No integration specs authored yet.)_ |
| `docs/changes/` | Change specs (`CHG-*`) and the change-spec template (`_TEMPLATE.md`). |

---

## System Spec Registry

All system specs are **Active** (greenfield, authoritative).

| Spec ID | Category | Title | Status |
|---|---|---|---|
| SDD-FORUM-001 | core | Comment Submission & Auto-Publish Flow | Active |
| SDD-FORUM-002 | core | Moderation Queue & Decisions | Active |
| SDD-FORUM-003 | core | Registration, Login & Account Activation | Active |
| SDD-FORUM-004 | core | Moderator Role Management | Active |
| SDD-FORUM-010 | domain | Comment Entity & Lifecycle | Active |
| SDD-FORUM-011 | domain | Roles & Authorization Model | Active |
| SDD-FORUM-020 | infrastructure | Comment Analysis (NAS-BERT; Strategy + Adapter + Object-Pool) | Active |
| SDD-FORUM-021 | infrastructure | Moderation Pipeline (Chain of Responsibility) | Active |
| SDD-FORUM-022 | infrastructure | Data Access (Repository + Unit of Work + Specification) | Active |
| SDD-FORUM-023 | infrastructure | Configuration & Options (ModerationOptions) | Active |
| SDD-FORUM-024 | infrastructure | ML Model Training & Packaging (ModelTrainer) | Active |

> All 11 system specs are authored and present in their category folders (4 core, 2 domain,
> 5 infrastructure). No integration specs have been authored. The `Key Files` paths in each spec
> remain PLANNED/TARGET; no `src/` code exists yet. Update rows here as a spec's status changes.

---

## Change Spec Registry

_No change specs yet._

| Change ID | Type | Title | Affected Specs | Status |
|---|---|---|---|---|
| _(none)_ | | | | |

---

## Status Legend

| Status | Meaning |
|---|---|
| **Active** | Authoritative and in force. Implementation and tests must conform. |
| **Draft** | Under authoring; not yet ratified. Not used for the initial greenfield specs. |
| **Deprecated** | Superseded or retired. Retained for history; no longer authoritative. |

---

## ID Allocation

| Category | Used range | Next free ID |
|---|---|---|
| core | 001–004 | **SDD-FORUM-005** |
| domain | 010–011 | **SDD-FORUM-012** |
| infrastructure | 020–024 | **SDD-FORUM-025** |
| integration | _(none)_ | **SDD-FORUM-001** |

> Allocate within a category's range. Never reuse an ID for unrelated behavior. After allocating a
> new ID, update both this table and the registry above.
