# Configuration & Options

| Field | Value |
|---|---|
| **Title** | Configuration & Options |
| **Spec ID** | SDD-FORUM-023 |
| **Category** | infrastructure |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

> Greenfield spec — **authoritative**. It defines what WILL be built. No production code exists yet;
> all paths under `src/` in *Key Files (planned)* are **PLANNED/TARGET** locations.

---

## 1. Context & Scope

This spec is the **single authority** for the ASP.NET Core Options-pattern configuration that
ForumGuard consumes across the system. It defines the `ModerationOptions` class, the configuration
keys it binds from, the binding registration, and — most importantly — the **fail-fast validation**
that runs at application startup so the app refuses to start when moderation configuration is invalid.

It also documents the `ConnectionStrings:ForumGuardDb` key as the configuration surface for the data
layer (the data layer itself is owned by **SDD-FORUM-022**).

### Patterns mandated by this spec

| Pattern | Where |
|---|---|
| **Options** | `ModerationOptions` bound from the `"Moderation"` configuration section. |
| **Options validation (fail-fast)** | DataAnnotations and/or `IValidateOptions<ModerationOptions>` registered with `ValidateOnStart()`. |
| **Hot-reload (Options Monitor)** | `IOptionsMonitor<ModerationOptions>` where a live (watched) value matters; `IOptions<ModerationOptions>` for one-time-read values. |

### Covered

- The **`ModerationOptions`** class and its four properties:
  - `ToxicityThreshold` (`float`, default `0.5`) — MUST validate within the inclusive range `[0.0, 1.0]`.
  - `ModelPath` (`string`) — MUST be non-empty.
  - `MaxCommentLength` (`int`) — MUST be positive; the domain cap is `4000`.
  - `ProfanityListPath` (`string`) — MUST be non-empty.
- **Binding** `ModerationOptions` from the configuration section `"Moderation"` via the Options pattern,
  registered in the Web/Infrastructure DI composition root.
- The **`ConnectionStrings:ForumGuardDb`** configuration key — documented here as the config surface
  consumed by the data layer (**SDD-FORUM-022**).
- **Fail-fast validation** of `ModerationOptions` at startup (`ValidateOnStart()`), so an invalid value
  prevents the application from starting with a clear, field-specific message. This is the behavior
  that **SDD-FORUM-021**'s error rule and **SDD-FORUM-020 §4** defer to.
- The authoritative **config-keys table** mapping each `appsettings` key to its `ModerationOptions`
  property, default, and constraint.

### Excluded (covered elsewhere)

- How `ToxicityThreshold` is *applied* to a model score to derive a `ToxicityLabel` — see
  **SDD-FORUM-020** (Comment Analysis). This spec only constrains the value, not its use.
- How `ProfanityListPath` / `ModelPath` files are *read or loaded* and the analyzer errors raised when
  they are missing at runtime — see **SDD-FORUM-020** (analyzer error rules) and **SDD-FORUM-021**
  (pipeline fail-closed behavior). This spec owns startup-time validation of the configured *values*,
  not runtime file I/O.
- How `MaxCommentLength` is enforced against a submitted `Comment.Body` — see **SDD-FORUM-001**
  (submission) and the field-length invariant in **SDD-FORUM-010**. This spec owns the option that
  carries the configured cap.
- The EF Core `DbContext`, schema, and use of the connection string — see **SDD-FORUM-022**. This spec
  only documents the `ConnectionStrings:ForumGuardDb` key as a config surface.
- The end-to-end submission and moderation flows — see **SDD-FORUM-001** / **SDD-FORUM-002**.

### Related specs (cross-referenced by ID)

- **SDD-FORUM-001** — Comment Submission & Auto-Publishing (consumes `MaxCommentLength`).
- **SDD-FORUM-020** — Comment Analysis (consumes `ToxicityThreshold`, `ModelPath`, `ProfanityListPath`;
  §4 defers analyzer-startup expectations to this spec).
- **SDD-FORUM-021** — Moderation Pipeline (its fail-safe error rule relies on this spec's fail-fast
  validation guaranteeing valid options at runtime).
- **SDD-FORUM-022** — Data Access (consumes `ConnectionStrings:ForumGuardDb`).

> **Spec-relationship note:** This spec (SDD-FORUM-023) is the **single authoritative source** for the
> `ModerationOptions` contract, its defaults, its constraints, and the fail-fast startup-validation
> behavior. Consumer specs reference these rules rather than restating them.

### Key Files (planned — code does not exist yet)

> All paths are **TARGET** locations. No source exists; these are created in Phase 2.

| Planned path | Responsibility |
|---|---|
| `src/ForumGuard.Application/Options/ModerationOptions.cs` | The `ModerationOptions` class (four properties + DataAnnotations). |
| `src/ForumGuard.Application/Options/ModerationOptionsValidator.cs` | `IValidateOptions<ModerationOptions>` implementation (range + non-empty + positive checks). |
| `src/ForumGuard.Application/Options/ModerationConfigKeys.cs` | String constants for the `"Moderation"` section and each key. |
| `src/ForumGuard.Web/Program.cs` | `AddOptions<ModerationOptions>().Bind(config.GetSection("Moderation")).ValidateDataAnnotations().Validate(...).ValidateOnStart()`. |
| `src/ForumGuard.Web/appsettings.json` | Declares the `Moderation:*` keys and `ConnectionStrings:ForumGuardDb`. |
| `tests/ForumGuard.Tests/Options/ModerationOptionsTests.cs` | `[Unit]` validation tests (each field, boundaries). |
| `tests/ForumGuard.Tests/Options/ModerationOptionsBindingTests.cs` | `[Integration]` binding + `ValidateOnStart` tests. |

---

## 2. Behavior

`ModerationOptions` is a plain options class bound from the `"Moderation"` configuration section. Its
default values and constraints below are independently testable against the validator and against a
configured DI container.

### 2.1 Happy path — valid configuration binds and the app starts

1. The application MUST register `ModerationOptions` against the configuration section named exactly
   `"Moderation"` using the ASP.NET Core Options pattern (e.g.
   `AddOptions<ModerationOptions>().Bind(configuration.GetSection("Moderation"))`).
2. When all four keys are present and valid, the bound `ModerationOptions` MUST expose:
   `ToxicityThreshold` from `Moderation:ToxicityThreshold`, `ModelPath` from `Moderation:ModelPath`,
   `MaxCommentLength` from `Moderation:MaxCommentLength`, and `ProfanityListPath` from
   `Moderation:ProfanityListPath`.
3. The registration MUST attach validation (`ValidateDataAnnotations()` and/or a registered
   `IValidateOptions<ModerationOptions>`) **and** MUST call `ValidateOnStart()` so validation runs at
   application startup rather than on first resolution.
4. When the configuration is valid, the host MUST start successfully and a resolved
   `IOptions<ModerationOptions>.Value` MUST return the bound, validated instance.
5. Consumers that need a single read at composition time SHOULD inject `IOptions<ModerationOptions>`;
   consumers that need the latest on-disk value (the analyzer reads `ModelPath` / `ProfanityListPath`,
   and the threshold may be operator-tuned) SHOULD inject `IOptionsMonitor<ModerationOptions>` so a
   configuration reload is observed without restart.
6. The `ConnectionStrings:ForumGuardDb` key MUST be the configuration surface consumed by the data
   layer (**SDD-FORUM-022**); it is read via `configuration.GetConnectionString("ForumGuardDb")` and is
   NOT a property of `ModerationOptions`.

### 2.2 Default values

7. `ToxicityThreshold` MUST default to `0.5` when the `Moderation:ToxicityThreshold` key is absent but
   the `"Moderation"` section otherwise binds. (The default is a property initializer on
   `ModerationOptions`, not a magic number in a consumer.)
8. `MaxCommentLength` SHOULD default to the domain cap `4000` when the key is absent; a bound value
   MUST NOT exceed `4000` (see §3) because `4000` is the `Comment.Body` length cap defined by
   **SDD-FORUM-010** and enforced at submission by **SDD-FORUM-001**.
9. `ModelPath` and `ProfanityListPath` have **no** safe default — they MUST be supplied; absence is a
   validation failure (§2.4), not a defaulted value.

### 2.3 Fail-fast validation at startup

10. Validation MUST be expressed as DataAnnotations on `ModerationOptions` and/or an
    `IValidateOptions<ModerationOptions>` implementation, and MUST be wired with `ValidateOnStart()`.
11. When any single field is invalid, application startup MUST fail (the host MUST throw
    `OptionsValidationException` during start) and MUST NOT serve requests with invalid moderation
    configuration. A `Clean`/`Toxic` decision MUST never run against an unvalidated threshold or path.
12. The validation failure message MUST identify which field failed and why (e.g.
    `"Moderation:ToxicityThreshold must be within [0.0, 1.0]."`), so the operator can correct the
    configuration without reading source code.
13. All field violations present at startup SHOULD be reported together (the validator SHOULD aggregate
    failures) rather than failing on only the first one.

### 2.4 Edge case — `"Moderation"` section missing entirely

14. When the `"Moderation"` configuration section is **absent**, binding MUST produce a
    `ModerationOptions` whose `ToxicityThreshold` is the default `0.5` and (per §2.2) `MaxCommentLength`
    is the default `4000`, but whose `ModelPath` and `ProfanityListPath` are null/empty.
15. Because `ModelPath` and `ProfanityListPath` are then empty, `ValidateOnStart()` MUST fail and the
    application MUST NOT start. A missing `"Moderation"` section MUST NOT be silently tolerated.

### 2.5 Edge case — `ToxicityThreshold` exactly `0.0` and exactly `1.0` (valid boundaries)

16. `ToxicityThreshold == 0.0` MUST be accepted as valid (lower inclusive boundary). Semantically every
    non-negative score then meets the threshold; this is a legal, intentional configuration.
17. `ToxicityThreshold == 1.0` MUST be accepted as valid (upper inclusive boundary). Only a perfect
    `1.0` toxic score then meets the threshold; this is also legal.
18. Any value strictly below `0.0` or strictly above `1.0` (e.g. `-0.01`, `1.5`) MUST be rejected by
    validation and MUST cause startup to fail (§2.3).

### 2.6 Edge case — partial configuration

19. When the `"Moderation"` section is present but **incomplete** (e.g. it sets only `ToxicityThreshold`
    and omits `ModelPath`), the provided keys MUST bind, the omitted-with-default keys MUST take their
    defaults (`ToxicityThreshold`, `MaxCommentLength`), and any omitted key that has no safe default
    (`ModelPath`, `ProfanityListPath`) MUST cause `ValidateOnStart()` to fail.
20. A partially valid configuration MUST NOT start the application in a half-configured state; either
    all required values are valid or the host fails fast.

---

## 3. Validation Rules

### 3.1 Field-level (`ModerationOptions`)

| Field | Type | Rule |
|---|---|---|
| `ToxicityThreshold` | `float` | Required after binding; MUST be within the inclusive range `[0.0, 1.0]`. Default `0.5`. Boundaries `0.0` and `1.0` are valid. |
| `ModelPath` | `string` | MUST be non-null and non-whitespace. No default. Existence of the file on disk is **not** checked here (runtime concern — see SDD-FORUM-020). |
| `MaxCommentLength` | `int` | MUST be a positive integer (`>= 1`); MUST NOT exceed `4000` (domain cap, SDD-FORUM-010). Default `4000`. |
| `ProfanityListPath` | `string` | MUST be non-null and non-whitespace. No default. File existence is a runtime concern (SDD-FORUM-020), not validated here. |

### 3.2 Cross-field rules

- **CF-1:** `MaxCommentLength` MUST be `1..4000` inclusive. A value of `0` or a negative value is
  invalid (non-positive); a value `> 4000` is invalid (exceeds the domain cap that `Comment.Body`
  enforces in SDD-FORUM-010 / SDD-FORUM-001).
- **CF-2:** No two options are interdependent; each field is validated independently. The validator
  MUST evaluate all four fields even when an earlier field already failed (§2.3 rule 13).

### 3.3 State-based / startup rules

- **SB-1:** Validation MUST execute at host startup via `ValidateOnStart()`; it MUST NOT be deferred to
  first option resolution. An invalid configuration MUST prevent the host from starting.
- **SB-2:** The `"Moderation"` section name and each key name MUST match the config-keys table in §3.4
  exactly (case-insensitive per ASP.NET Core configuration semantics). A key under a different section
  MUST NOT bind.
- **SB-3:** `ConnectionStrings:ForumGuardDb` MUST be present for the data layer to start
  (SDD-FORUM-022); its absence is a data-layer startup failure, documented here as the owning config
  surface but enforced by SDD-FORUM-022.

### 3.4 Config-keys table (authoritative)

| Configuration key | `ModerationOptions` property | Type | Default | Constraint |
|---|---|---|---|---|
| `Moderation:ToxicityThreshold` | `ToxicityThreshold` | `float` | `0.5` | Inclusive range `[0.0, 1.0]`. |
| `Moderation:ModelPath` | `ModelPath` | `string` | _(none)_ | Non-empty / non-whitespace. |
| `Moderation:MaxCommentLength` | `MaxCommentLength` | `int` | `4000` | Positive (`>= 1`) and `<= 4000`. |
| `Moderation:ProfanityListPath` | `ProfanityListPath` | `string` | _(none)_ | Non-empty / non-whitespace. |
| `ConnectionStrings:ForumGuardDb` | _(not on `ModerationOptions`)_ | `string` | _(none)_ | Non-empty; consumed by the data layer (SDD-FORUM-022). |

---

## 4. Error Rules

All validation errors below are **startup-time, fail-fast** errors. There is no API boundary; the
host's start sequence raises the error and the application does not begin serving requests. The
implementer MAY surface them as the standard `OptionsValidationException` produced by
`ValidateOnStart()` (a `Result`-style aggregation is acceptable so long as the host still aborts start).

| # | Trigger | Type | Domain mapping | User-facing / operator outcome |
|---|---|---|---|---|
| ER-1 | `Moderation:ToxicityThreshold` is `< 0.0` or `> 1.0` | validation (startup) | `OptionsValidationException` listing `ToxicityThreshold`; app does not start | Startup logs a clear message naming the field and the `[0.0, 1.0]` constraint; operator fixes `appsettings`. |
| ER-2 | `Moderation:ModelPath` is missing/empty/whitespace | validation (startup) | `OptionsValidationException` listing `ModelPath`; app does not start | Startup fails with "Moderation:ModelPath must be a non-empty path."; app never runs analysis against an undefined model. |
| ER-3 | `Moderation:MaxCommentLength` is `<= 0` | validation (startup) | `OptionsValidationException` listing `MaxCommentLength`; app does not start | Startup fails with "Moderation:MaxCommentLength must be a positive integer." |
| ER-4 | `Moderation:MaxCommentLength` is `> 4000` (exceeds domain cap) | validation (startup) | `OptionsValidationException` listing `MaxCommentLength`; app does not start | Startup fails with "Moderation:MaxCommentLength must not exceed the domain cap of 4000." |
| ER-5 | `Moderation:ProfanityListPath` is missing/empty/whitespace | validation (startup) | `OptionsValidationException` listing `ProfanityListPath`; app does not start | Startup fails with "Moderation:ProfanityListPath must be a non-empty path." |
| ER-6 | `"Moderation"` section absent entirely | validation (startup) | `OptionsValidationException` aggregating `ModelPath` + `ProfanityListPath` (defaults cover threshold/length) | Startup fails listing the missing required paths; app never starts half-configured (§2.4). |
| ER-7 | `ConnectionStrings:ForumGuardDb` absent/empty | configuration (startup) | Data-layer startup failure (owned by SDD-FORUM-022); documented surface here | Startup fails when the data layer initializes; operator supplies the connection string. |

**Fail-fast principle (MUST):** an invalid `ModerationOptions` MUST NEVER be silently corrected,
clamped, or defaulted past its constraint. Any field violation (ER-1…ER-6) MUST abort host startup so
no moderation decision can run against invalid configuration.

---

## 5. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Defines the `ModerationOptions` class
  (`ToxicityThreshold` default `0.5` in `[0.0, 1.0]`; `ModelPath` non-empty; `MaxCommentLength`
  positive and `<= 4000`; `ProfanityListPath` non-empty), binding from the `"Moderation"` section via
  the Options pattern (`IOptions` / `IOptionsMonitor`), the `ConnectionStrings:ForumGuardDb` config
  surface, fail-fast validation via DataAnnotations and/or `IValidateOptions<ModerationOptions>` with
  `ValidateOnStart()`, the authoritative config-keys table, and the missing-section / boundary-value
  (`0.0` and `1.0`) / partial-config edge cases. Non-breaking (greenfield first version).

---

## 6. Test Plan

Business tests reference this spec via `[Category("SDD-FORUM-023")]`. Test names follow
`MethodName_Scenario_ExpectedResult`. This spec crosses the configuration / DI startup boundary, so it
carries **both** `[Unit]` (validator in isolation) and `[Integration]` (binding + `ValidateOnStart`
through a host) tests.

```
Required tests:

# Unit — validator in isolation (each field, valid + invalid)
- Validate_AllFieldsValid_ReturnsSuccess                                       [Unit]
- Validate_ToxicityThresholdBelowZero_ReturnsFailure                           [Unit]
- Validate_ToxicityThresholdAboveOne_ReturnsFailure                            [Unit]
- Validate_ToxicityThresholdExactlyZero_ReturnsSuccess                         [Unit]
- Validate_ToxicityThresholdExactlyOne_ReturnsSuccess                          [Unit]
- Validate_ModelPathEmptyOrWhitespace_ReturnsFailure                           [Unit]
- Validate_ProfanityListPathEmptyOrWhitespace_ReturnsFailure                   [Unit]
- Validate_MaxCommentLengthZeroOrNegative_ReturnsFailure                       [Unit]
- Validate_MaxCommentLengthExceeds4000_ReturnsFailure                          [Unit]
- Validate_MaxCommentLengthAt4000_ReturnsSuccess                               [Unit]
- Validate_MultipleInvalidFields_AggregatesAllFailures                         [Unit]
- Validate_FailureMessage_NamesOffendingFieldAndConstraint                     [Unit]

# Unit — defaults
- ModerationOptions_DefaultToxicityThreshold_IsZeroPointFive                   [Unit]
- ModerationOptions_DefaultMaxCommentLength_Is4000                             [Unit]

# Integration — binding from configuration + ValidateOnStart through the host
- BindModerationSection_ValidConfig_PopulatesAllProperties                     [Integration]
- BindModerationSection_AbsentThresholdKey_AppliesDefaultZeroPointFive         [Integration]
- BindConnectionString_ForumGuardDbKey_IsReadableViaGetConnectionString        [Integration]
- ValidateOnStart_ValidConfig_HostStartsSuccessfully                           [Integration]
- ValidateOnStart_ToxicityThresholdOutOfRange_ThrowsOptionsValidationException [Integration]
- ValidateOnStart_EmptyModelPath_ThrowsOptionsValidationException              [Integration]
- ValidateOnStart_EmptyProfanityListPath_ThrowsOptionsValidationException      [Integration]
- ValidateOnStart_NonPositiveMaxCommentLength_ThrowsOptionsValidationException [Integration]
- ValidateOnStart_MissingModerationSection_ThrowsOptionsValidationException    [Integration]
- ValidateOnStart_PartialConfigMissingRequiredPath_FailsHostStart             [Integration]
- OptionsMonitor_ConfigReloaded_ObservesUpdatedThresholdWithoutRestart         [Integration]
```
