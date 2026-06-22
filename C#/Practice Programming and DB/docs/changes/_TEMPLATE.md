# CHG-<TYPE>-<NNN> — <Short Title>

> Use this template for **change specs**. A change spec records a single, scoped modification
> to one or more existing system specs (`SDD-FORUM-*`). It does **not** replace a system spec —
> it amends it and links back to it.
>
> Copy this file, rename it `CHG-<TYPE>-<NNN>-<short-desc>.md`, and fill in every section.
> Delete these instruction blockquotes before committing.

## Change Type Prefixes

| Prefix | Use for |
|---|---|
| `CHG-FEAT`  | A new feature or capability added to the product |
| `CHG-ENH`   | An enhancement / behavioral improvement to an existing feature |
| `CHG-FIX`   | A correction of incorrect documented or implemented behavior |
| `CHG-REFAC` | A refactor that preserves behavior but changes structure/design |
| `CHG-DEBT`  | Technical-debt paydown (cleanup, dependency upgrades, deprecations) |

---

## 1. Header

| Field | Value |
|---|---|
| **Title** | <Short, descriptive title> |
| **Change ID** | CHG-<TYPE>-<NNN> |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |
| **Supersedes / Relates to** | <prior change IDs, or "None"> |

---

## 2. Context & Scope

- **Why this change is needed:** <problem statement / motivation>
- **In scope:** <what this change covers>
- **Out of scope:** <what this change explicitly does NOT cover>
- **Related system specs:** <SDD-FORUM-NNN, ...> (reference by ID)

---

## 3. Behavior (RFC 2119)

> Describe the changed behavior. Happy path first, then at least two edge cases.
> Every MUST / SHOULD statement must be independently testable.

- The system **MUST** ...
- The system **SHOULD** ...
- The system **MAY** ...

**Edge case 1:** ...

**Edge case 2:** ...

---

## 4. Validation Rules

- **Field-level:** <required, max length, format, range>
- **Cross-field:** <field A required when field B = X>
- **State-based:** <forbidden / allowed state transitions>

---

## 5. Error Rules

| Trigger | Error Type | Domain Mapping | User-Facing Outcome (Razor) |
|---|---|---|---|
| <condition> | validation / conflict / notfound / authorization | <exception type or `Result<T>` error> | <page result / message> |

---

## 6. Versioning Notes

- **v1 — Initial change specification.** <breaking / non-breaking + reasoning>

---

## 7. Test Plan

> Use the convention `MethodName_Scenario_ExpectedResult`. Tag each `[Unit]` or `[Integration]`.

Required tests:
- `Method_Scenario_ExpectedResult`            [Unit]
- `Method_Scenario_ExpectedResult`            [Integration]

---

## 8. Detailed Design

> Describe the concrete design: affected classes, interfaces, patterns, data flow,
> new/changed methods. Reference the design patterns from the project pattern catalog
> (Strategy, Adapter, Object-Pool, Repository, Unit of Work, Specification,
> Chain of Responsibility, custom authorization Strategy, IRoleWorkspace Strategy, Options).

---

## 9. Affected System Specs

| Spec ID | Category | Nature of Impact |
|---|---|---|
| SDD-FORUM-NNN | core / domain / infrastructure / integration | <amended behavior / new validation / new error rule> |

---

## 10. Migration Plan

> Required when the change touches the database (EF Core + MSSQL) or persisted data.
> New migration file name (`v[X.Y.Z]_[ShortDescription].sql`), Fluent API config changes,
> data backfill steps, rollback considerations. Write "No database impact" if none.

---

## 11. Open Questions

- <question 1>
- <question 2>
