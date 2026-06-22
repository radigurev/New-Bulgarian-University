# SDD-FORUM-010 — Comment Entity & Lifecycle

## 1. Header

| Field | Value |
|---|---|
| **Title** | Comment Entity & Lifecycle |
| **Spec ID** | SDD-FORUM-010 |
| **Category** | domain |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

> **Greenfield note:** No production code exists yet. This spec is **authoritative** — it defines
> what the `Comment` entity and its `CommentStatus` state machine MUST be. "Key Files" paths are
> **planned/target** locations under `src/`; they do not exist at the time of writing.

---

## 2. Context & Scope

### Covered

- The **`Comment`** entity: its fields, their types, nullability, and field-level invariants.
- The **`CommentStatus`** state machine: the complete set of states, the allowed transitions
  between them as a transition table, the terminal states, and the side effects each transition MUST apply.
- The **visibility invariant**: the single rule that determines whether a comment is publicly visible.
- The **transition guard contract**: a domain-level decision function (no I/O) that, given a current
  status and a requested target status, returns whether the transition is allowed and forbids all others.

### Excluded

- The orchestration that *triggers* transitions (submission, analysis scoring, moderator decisions).
  That is application/feature behavior — see the cross-referenced specs below.
- ML scoring, the `ToxicityThreshold` comparison, and `AnalysisLabel`/`AnalysisScore` population logic
  (this spec only declares the fields and constrains *when* they MUST be set, not *how* they are computed).
- Persistence concerns: EF Core mapping, table/column naming, GUID generation (`NEWSEQUENTIALID()`),
  and the `ModerationDecision` audit record.
- Authorization (who may request a transition).

### Related specs (cross-referenced by ID)

- **SDD-FORUM-001** — Comment Submission & Auto-Publish Flow. Drives `PendingAnalysis → Published`
  and `PendingAnalysis → FlaggedForReview`.
- **SDD-FORUM-002** — Moderation Queue & Decisions. Drives `FlaggedForReview → ApprovedByModerator`
  and `FlaggedForReview → RejectedByModerator` when a moderator decides.
- **SDD-FORUM-011** — Roles & Authorization Model (authorization policies and handlers; does NOT
  restate the comment lifecycle).
- **SDD-FORUM-020** — Comment Analysis (Strategy + Adapter + Object-Pool). Supplies the
  `AnalysisResult { ToxicityLabel Label, float Score }` consumed when leaving `PendingAnalysis`.
- **SDD-FORUM-021** — Moderation Pipeline (Chain of Responsibility). Produces the analysis result
  that selects the post-analysis target state.
- **SDD-FORUM-022** — Data Access (Repository + Unit of Work + Specification). Persists `Comment`
  and exposes specifications such as the visibility predicate defined here.

> **Spec-relationship note:** This spec (SDD-FORUM-010) is the **single authoritative source** for the
> `Comment` entity field contract, the `CommentStatus` state machine, and the visibility invariant.
> No other spec restates the lifecycle; specs that need it cross-reference SDD-FORUM-010.

---

## 3. Behavior

The `Comment` lifecycle is a deterministic state machine over the enum
`CommentStatus { PendingAnalysis, Published, FlaggedForReview, ApprovedByModerator, RejectedByModerator }`.
All MUST/SHOULD rules below are independently testable against a pure domain function with no I/O.

### 3.1 Happy path — Clean comment auto-publishes

1. A newly constructed `Comment` MUST have `Status == CommentStatus.PendingAnalysis`, `PublishedAtUtc == null`,
   `AnalysisLabel == null`, `AnalysisScore == null`, and `AnalyzedAtUtc == null`.
2. When analysis classifies the comment as `ToxicityLabel.Clean`, the domain MUST allow the transition
   `PendingAnalysis → Published`.
3. On entering `Published`, the comment MUST set `PublishedAtUtc` to the current UTC instant
   (`SYSUTCDATETIME()` semantics) and MUST set `AnalyzedAtUtc`, `AnalysisLabel` (`Clean`), and
   `AnalysisScore` to the analysis values.
4. A comment in `Published` MUST be publicly visible (per the visibility invariant in §3.6).
5. `Published` is **terminal** — no further transition out of `Published` is allowed.

### 3.2 Happy path — Toxic comment routes to moderation, then is approved

1. When analysis classifies the comment as `ToxicityLabel.Toxic`, the domain MUST allow the transition
   `PendingAnalysis → FlaggedForReview`.
2. On entering `FlaggedForReview`, the comment MUST set `AnalyzedAtUtc`, `AnalysisLabel` (`Toxic`), and
   `AnalysisScore`, and MUST leave `PublishedAtUtc == null`.
3. A comment in `FlaggedForReview` MUST NOT be publicly visible and MUST appear in the moderator queue.
4. The domain MUST allow `FlaggedForReview → ApprovedByModerator` (moderator approves) and
   `FlaggedForReview → RejectedByModerator` (moderator rejects).
5. On entering `ApprovedByModerator`, the comment MUST set `PublishedAtUtc` to the current UTC instant
   and MUST become publicly visible.
6. On entering `RejectedByModerator`, the comment MUST leave `PublishedAtUtc == null` and MUST remain hidden.
7. `ApprovedByModerator` and `RejectedByModerator` are both **terminal**.

### 3.3 Allowed-transition table (authoritative)

The following table is the **complete** set of allowed transitions. Any (from, to) pair not listed
MUST be rejected (see §3.4 and §5).

| From | To | Guard / trigger | Side effects on entering target |
|---|---|---|---|
| _(construction)_ | `PendingAnalysis` | New comment created | `CreatedAtUtc` set; all analysis fields and `PublishedAtUtc` null |
| `PendingAnalysis` | `Published` | analysis result = `Clean` | set `PublishedAtUtc`, `AnalyzedAtUtc`, `AnalysisLabel=Clean`, `AnalysisScore` |
| `PendingAnalysis` | `FlaggedForReview` | analysis result = `Toxic` | set `AnalyzedAtUtc`, `AnalysisLabel=Toxic`, `AnalysisScore`; `PublishedAtUtc` stays null |
| `FlaggedForReview` | `ApprovedByModerator` | moderator `ModerationOutcome.Approved` | set `PublishedAtUtc` |
| `FlaggedForReview` | `RejectedByModerator` | moderator `ModerationOutcome.Rejected` | `PublishedAtUtc` stays null |

**Terminal states (no outgoing transition):** `Published`, `ApprovedByModerator`, `RejectedByModerator`.

The domain MUST expose a pure guard (planned: `CommentStatusTransitions.IsAllowed(from, to)` returning
`bool`) that returns `true` for exactly the five rows above and `false` for every other ordered pair,
including `(X, X)` self-transitions.

### 3.4 Edge case — Illegal transition attempt is rejected

- Requesting `FlaggedForReview → Published` directly (skipping a moderator decision) MUST be rejected.
  A flagged comment can become visible **only** via `ApprovedByModerator`.
- Requesting `PendingAnalysis → ApprovedByModerator` or `PendingAnalysis → RejectedByModerator`
  (skipping analysis routing) MUST be rejected.
- Requesting any transition **into** `PendingAnalysis` from a non-construction state MUST be rejected.
- A rejected transition MUST NOT mutate the comment: `Status` and all timestamps/analysis fields MUST
  be unchanged after a rejected attempt.

### 3.5 Edge case — Re-analysis / re-decision not allowed once terminal

- Once a comment is in any terminal state (`Published`, `ApprovedByModerator`, `RejectedByModerator`),
  the domain MUST reject every further transition request, including re-running analysis or re-deciding.
- A comment already in `Published` MUST NOT be moved to `FlaggedForReview` (a published comment is never
  re-flagged by this domain rule).
- A comment in `RejectedByModerator` MUST NOT be moved to `ApprovedByModerator` (no moderator "undo"
  at the domain level; a fresh comment would be required instead).
- Setting `AnalysisLabel`/`AnalysisScore`/`AnalyzedAtUtc` after a comment has left `PendingAnalysis`
  MUST NOT change `Status` and MUST NOT re-trigger routing.

### 3.6 Visibility invariant

- A comment is publicly visible **IFF** `Status == CommentStatus.Published` **OR**
  `Status == CommentStatus.ApprovedByModerator`. The domain MUST expose this as a single predicate
  (planned: `Comment.IsPubliclyVisible`) that returns `true` only for those two states and `false` for
  `PendingAnalysis`, `FlaggedForReview`, and `RejectedByModerator`.
- The domain SHOULD guarantee `PublishedAtUtc != null` whenever `IsPubliclyVisible` is `true`, and
  `PublishedAtUtc == null` for every non-visible state.

### Key Files (planned — code does not exist yet)

- `src/ForumGuard.Domain/Entities/Comment.cs` — the `Comment` entity (fields + `IsPubliclyVisible`).
- `src/ForumGuard.Domain/Enums/CommentStatus.cs` — the `CommentStatus` enum.
- `src/ForumGuard.Domain/Enums/ToxicityLabel.cs` — the `ToxicityLabel` enum.
- `src/ForumGuard.Domain/Enums/ModerationOutcome.cs` — the `ModerationOutcome` enum (decision input).
- `src/ForumGuard.Domain/Lifecycle/CommentStatusTransitions.cs` — pure transition guard (`IsAllowed`).
- `tests/ForumGuard.Tests/Domain/CommentLifecycleTests.cs` — unit tests for this spec.

---

## 4. Validation Rules

### 4.1 Field-level (`Comment`)

| Field | Type | Rule |
|---|---|---|
| `Id` | `Guid` | Primary key; MUST be non-empty (`Guid.Empty` is invalid). DB default `NEWSEQUENTIALID()`. |
| `ThreadId` | `Guid` | Required FK → `ForumThread`; MUST be non-empty. |
| `AuthorId` | `Guid` | Required FK → `ApplicationUser`; MUST be non-empty. |
| `Body` | `string` | Required; MUST be non-null, non-whitespace; length MUST be `1..4000`. |
| `Status` | `CommentStatus` | MUST be a defined enum member; a new comment MUST start at `PendingAnalysis`. |
| `CreatedAtUtc` | `DateTime` (DATETIME2(7)) | Required; MUST be a UTC instant set at construction. |
| `PublishedAtUtc` | `DateTime?` | Nullable; set only on entering `Published` or `ApprovedByModerator`. |
| `AnalysisLabel` | `ToxicityLabel?` | Nullable; set when leaving `PendingAnalysis`; a defined enum member when non-null. |
| `AnalysisScore` | `float?` | Nullable; when non-null MUST be within `0.0..1.0` inclusive. |
| `AnalyzedAtUtc` | `DateTime?` | Nullable; set when leaving `PendingAnalysis`. |

### 4.2 Cross-field rules

- **CF-1:** If `IsPubliclyVisible` is `true`, then `PublishedAtUtc` MUST be non-null.
- **CF-2:** If `Status == PendingAnalysis`, then `AnalysisLabel`, `AnalysisScore`, `AnalyzedAtUtc`, and
  `PublishedAtUtc` MUST all be null.
- **CF-3:** If `Status` is any non-`PendingAnalysis` state, then `AnalysisLabel`, `AnalysisScore`, and
  `AnalyzedAtUtc` MUST all be non-null (analysis happened before leaving `PendingAnalysis`).
- **CF-4:** If `Status == FlaggedForReview` or `Status == RejectedByModerator`, then `PublishedAtUtc`
  MUST be null (these states are not visible).
- **CF-5:** If `AnalysisLabel == Clean` was the routing decision, the resulting state MUST be `Published`,
  not `FlaggedForReview`; if `AnalysisLabel == Toxic`, the resulting state MUST be `FlaggedForReview`,
  not `Published`.

### 4.3 State-based rules

- **SB-1:** Only the five (from, to) pairs in §3.3 are valid transitions; all others are invalid.
- **SB-2:** The three terminal states have no valid outgoing transition.
- **SB-3:** Self-transitions `(X → X)` are invalid for every state.
- **SB-4:** No state other than the construction origin may transition **into** `PendingAnalysis`.

---

## 5. Error Rules

This is a **domain** spec; transitions are evaluated by a pure domain function with no API boundary.
Errors surface as a domain failure (the implementer MAY use a thrown exception such as
`InvalidCommentTransitionException` or a `Result`-style return; both are acceptable so long as the
guard `IsAllowed` remains a side-effect-free `bool`).

| # | Trigger | Type | Domain mapping | User-facing outcome (when surfaced via a Razor handler) |
|---|---|---|---|---|
| ER-1 | Requested (from, to) pair not in §3.3 (e.g., `FlaggedForReview → Published`) | conflict (invalid state transition) | `InvalidCommentTransitionException` **or** failed `Result` with an `InvalidTransition` error; comment is left unchanged | Action is refused; the page re-renders with a "this action is not allowed for the comment's current status" message — no state change persisted. |
| ER-2 | Transition requested from a terminal state (re-analysis / re-decision) | conflict | Same as ER-1 (`InvalidCommentTransitionException` / failed `Result`) | Refused; user/moderator informed the comment is already finalized. |
| ER-3 | `Body` null/whitespace or length outside `1..4000` | validation | `ArgumentException` / validation failure at construction; comment is not created | Submission rejected; form shows a field validation message (see SDD-FORUM-001). |
| ER-4 | `AnalysisScore` set outside `0.0..1.0` | validation | `ArgumentOutOfRangeException` / validation failure | Internal error; not normally reachable from user input (analysis is system-produced). |
| ER-5 | `Status` set to an undefined `CommentStatus` value | validation | `ArgumentException` / `InvalidEnumArgumentException` | Internal error; not user-reachable. |

A rejected transition (ER-1, ER-2) MUST be **atomic**: the comment's observable state MUST be identical
before and after the failed attempt.

---

## 6. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Establishes the `Comment` field contract, the complete
  `CommentStatus` allowed-transition table, the three terminal states, the visibility invariant, the
  illegal-transition and post-terminal re-analysis rejection rules, and the cross-field/state-based
  validation rules. Non-breaking (greenfield first version).

---

## 7. Test Plan

All tests target the pure domain transition guard and `Comment` invariants — no database, ML model,
or DI container is involved — so this spec requires **unit tests only**. Every test below references
this spec via `[Category("SDD-FORUM-010")]`.

```
Required tests:

# Construction & initial state
- NewComment_AfterConstruction_StatusIsPendingAnalysis                         [Unit]
- NewComment_AfterConstruction_AnalysisFieldsAndPublishedAtAreNull             [Unit]
- NewComment_BodyNullOrWhitespace_FailsValidation                              [Unit]
- NewComment_BodyExceeds4000Chars_FailsValidation                              [Unit]
- NewComment_BodyAt4000Chars_PassesValidation                                  [Unit]

# Allowed transitions (happy paths)
- IsAllowed_PendingAnalysisToPublished_ReturnsTrue                             [Unit]
- IsAllowed_PendingAnalysisToFlaggedForReview_ReturnsTrue                      [Unit]
- IsAllowed_FlaggedForReviewToApprovedByModerator_ReturnsTrue                  [Unit]
- IsAllowed_FlaggedForReviewToRejectedByModerator_ReturnsTrue                  [Unit]

# Transition side effects
- TransitionToPublished_CleanAnalysis_SetsPublishedAtAndAnalysisFields         [Unit]
- TransitionToFlaggedForReview_ToxicAnalysis_LeavesPublishedAtNull             [Unit]
- TransitionToApprovedByModerator_FromFlagged_SetsPublishedAt                  [Unit]
- TransitionToRejectedByModerator_FromFlagged_LeavesPublishedAtNull            [Unit]

# Illegal transitions (edge case 3.4)
- IsAllowed_FlaggedForReviewToPublished_ReturnsFalse                           [Unit]
- IsAllowed_PendingAnalysisToApprovedByModerator_ReturnsFalse                  [Unit]
- IsAllowed_PendingAnalysisToRejectedByModerator_ReturnsFalse                  [Unit]
- IsAllowed_AnyStateToPendingAnalysis_ReturnsFalse                             [Unit]
- IsAllowed_SelfTransition_ReturnsFalseForEveryState                           [Unit]
- ApplyTransition_IllegalPair_LeavesCommentUnchanged                           [Unit]
- ApplyTransition_IllegalPair_RaisesInvalidCommentTransition                   [Unit]

# Terminal / re-analysis rejection (edge case 3.5)
- IsAllowed_FromPublished_ReturnsFalseForEveryTarget                           [Unit]
- IsAllowed_FromApprovedByModerator_ReturnsFalseForEveryTarget                 [Unit]
- IsAllowed_FromRejectedByModerator_ReturnsFalseForEveryTarget                 [Unit]
- ApplyTransition_PublishedToFlaggedForReview_IsRejected                       [Unit]
- ApplyTransition_RejectedToApproved_IsRejected                                [Unit]

# Visibility invariant (3.6)
- IsPubliclyVisible_StatusPublished_ReturnsTrue                                [Unit]
- IsPubliclyVisible_StatusApprovedByModerator_ReturnsTrue                      [Unit]
- IsPubliclyVisible_StatusPendingAnalysis_ReturnsFalse                         [Unit]
- IsPubliclyVisible_StatusFlaggedForReview_ReturnsFalse                        [Unit]
- IsPubliclyVisible_StatusRejectedByModerator_ReturnsFalse                     [Unit]
- IsPubliclyVisible_True_ImpliesPublishedAtNotNull                             [Unit]

# Cross-field / state-based validation (§4.2 / §4.3)
- Validate_PendingAnalysisWithAnalysisFieldsSet_FailsCF2                       [Unit]
- Validate_NonPendingStatusWithNullAnalysisFields_FailsCF3                     [Unit]
- Validate_FlaggedForReviewWithPublishedAtSet_FailsCF4                         [Unit]
- Validate_AnalysisScoreOutOfRange_FailsValidation                            [Unit]
- Validate_CleanLabelRoutedToFlaggedForReview_FailsCF5                         [Unit]
```
