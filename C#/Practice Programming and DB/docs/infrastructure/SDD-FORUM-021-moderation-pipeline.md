# Moderation Pipeline (Chain of Responsibility)

| Field | Value |
|---|---|
| **Title** | Moderation Pipeline (Chain of Responsibility) |
| **Spec ID** | SDD-FORUM-021 |
| **Category** | infrastructure |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 1. Context & Scope

This spec defines the **moderation pipeline** that decides, for a single comment, whether it is
auto-published (`Clean`) or withheld for human review (`Toxic`). The pipeline is implemented as a
**Chain of Responsibility (CoR)** of `ICommentModerationHandler` links. Each link inspects the
comment under analysis and either **flags** it (short-circuits the chain) or **passes** control to
the next link. If every link passes, the comment is `Clean` and is auto-published.

The canonical chain order is:

```
ProfanityPreFilterHandler  →  MlToxicityHandler  →  (extensible: length / spam / …)
```

- **`ProfanityPreFilterHandler`** uses `KeywordCommentAnalyzer` (a fast, model-free `ICommentAnalyzer`
  Strategy). Obvious profanity short-circuits the chain to `Toxic` **without invoking the ML model**.
- **`MlToxicityHandler`** uses the NAS-BERT `ICommentAnalyzer` Strategy (`NasBertCommentAnalyzer`).
  A toxic score `≥ Moderation:ToxicityThreshold` short-circuits the chain to `Toxic`.
- The chain is **extensible** and its **order is configurable**.

### Covered

- The `ICommentModerationHandler` contract and the chain-assembly / traversal contract.
- The two seed handlers (`ProfanityPreFilterHandler`, `MlToxicityHandler`) and the extensibility contract for further handlers.
- Short-circuit-on-flag semantics, pass-through semantics, and the empty-chain default.
- Fail-safe behavior when a handler throws.
- The `ModerationVerdict` result returned by the pipeline (flag vs. pass; first-flagging handler; label and score).

### Excluded

- The HTTP/Razor request flow that submits a comment and persists the resulting status — see **SDD-FORUM-001** (Comment Submission & Auto-Publish Flow).
- The concrete `ICommentAnalyzer` implementations (Strategy + Adapter + Object-Pool, NAS-BERT model loading) — see **SDD-FORUM-020 (Comment Analysis)**, registered separately; the analyzers are consumed here as Strategy dependencies only. *(Pipeline consumes `ICommentAnalyzer`; it does not define it.)*
- The comment lifecycle state machine and the `CommentStatus` / `ToxicityLabel` enums — see **SDD-FORUM-010** (Comment Entity & Lifecycle).
- The moderator review-queue UI and Approve/Reject decisions for flagged comments — see **SDD-FORUM-002** (Moderation Queue & Decisions).

### Related specs

| Spec ID | Relationship |
|---|---|
| **SDD-FORUM-001** | Caller of this pipeline; maps a `ModerationVerdict` onto `CommentStatus` (`Published` vs. `FlaggedForReview`) and persists it. |
| **SDD-FORUM-010** | Owns the `Comment` entity, `CommentStatus` / `ToxicityLabel` enums, and the lifecycle invariants this pipeline must respect. |
| **SDD-FORUM-020 (Comment Analysis)** | Provides the `ICommentAnalyzer` Strategy implementations (`KeywordCommentAnalyzer`, `NasBertCommentAnalyzer`) this pipeline orchestrates. |
| **SDD-FORUM-023** | Provides `ModerationOptions` (`ToxicityThreshold`, `MaxCommentLength`, `ProfanityListPath`) consumed by the handlers. |

> **Cross-reference note.** This document (SDD-FORUM-021) defines the **pipeline mechanism**: the
> Chain-of-Responsibility handler chain that decides which comments are flagged. It is **invoked by
> SDD-FORUM-001** (Comment Submission & Auto-Publish Flow), which maps the resulting `ModerationVerdict`
> onto `CommentStatus` and persists it. **SDD-FORUM-002** (Moderation Queue & Decisions) is the
> *complementary* downstream spec: it handles the moderator's review of comments this pipeline has
> already flagged (`FlaggedForReview` → Approve/Reject). In short: SDD-FORUM-021 flags comments;
> SDD-FORUM-002 handles moderator action on the flagged comments.

---

## 2. Behavior

The pipeline exposes a single asynchronous entry point that evaluates one comment's `Body`:

```
Task<ModerationVerdict> EvaluateAsync(CommentModerationContext context, CancellationToken ct)
```

`CommentModerationContext` carries the comment `Body` (and, optionally, the `Comment.Id` for
traceability). `ModerationVerdict` reports `{ ToxicityLabel Label, float Score, bool IsFlagged,
string? FlaggingHandlerName }` where `IsFlagged == (Label == ToxicityLabel.Toxic)`.

### 2.1 Happy path — clean comment traverses the whole chain and publishes

1. The pipeline MUST invoke the registered handlers **in configured order**, starting at the head of the chain.
2. `ProfanityPreFilterHandler` MUST run `KeywordCommentAnalyzer`; when no profanity keyword matches it MUST **pass** to the next handler.
3. `MlToxicityHandler` MUST run the NAS-BERT `ICommentAnalyzer`; when the toxic score is `< Moderation:ToxicityThreshold` it MUST **pass** to the next handler.
4. When **all** handlers pass, the pipeline MUST return a verdict with `Label = ToxicityLabel.Clean`, `IsFlagged = false`, and `FlaggingHandlerName = null`.
5. The reported `Score` MUST be the toxic score produced by the last analyzing handler that ran (i.e. the ML toxic score in the canonical chain).
6. A `Clean` verdict MUST cause the caller (SDD-FORUM-001) to set the comment to `CommentStatus.Published`; this spec MUST NOT itself mutate or persist the `Comment`.

### 2.2 Edge case A — profanity short-circuits at the first link without invoking ML

1. When `ProfanityPreFilterHandler` detects a profanity keyword, it MUST **flag** and **short-circuit**: no subsequent handler runs.
2. In this case the NAS-BERT `ICommentAnalyzer` MUST NOT be invoked (verifiable: the ML analyzer receives zero calls).
3. The returned verdict MUST have `Label = ToxicityLabel.Toxic`, `IsFlagged = true`, and `FlaggingHandlerName = nameof(ProfanityPreFilterHandler)`.
4. A flagged verdict MUST cause the caller to set the comment to `CommentStatus.FlaggedForReview` (hidden; routed to the moderator queue).

### 2.3 Edge case B — empty chain defaults to publish

1. When the pipeline is assembled with **no** handlers, `EvaluateAsync` MUST return `Label = ToxicityLabel.Clean`, `IsFlagged = false`, `Score = 0`, `FlaggingHandlerName = null`.
2. An empty chain MUST NOT throw.

### 2.4 Edge case C — a handler throws (fail-safe)

1. If any handler throws while evaluating, the pipeline MUST **fail safe**: it MUST stop the chain and return a **flagged** verdict (`Label = ToxicityLabel.Toxic`, `IsFlagged = true`, `FlaggingHandlerName = <the throwing handler's name>`).
2. A handler exception MUST NOT propagate out of `EvaluateAsync` (it MUST be caught and converted to a fail-safe flag).
3. The pipeline MUST log the underlying exception (handler name + comment id) before returning the fail-safe verdict.
4. Failing safe MUST route the comment to `CommentStatus.FlaggedForReview`, never to `Published`. (A scoring/model failure must never auto-publish potentially toxic content.)

### 2.5 General mechanism rules

1. Each `ICommentModerationHandler` MUST return one of exactly two outcomes per evaluation: **Flag** (short-circuit) or **Pass** (continue).
2. A handler that flags MUST set the verdict `Label = ToxicityLabel.Toxic` and supply its own name as `FlaggingHandlerName`; the chain MUST stop immediately after the first flagging handler.
3. The pipeline MUST be assembled in the order defined by configuration; reordering handlers in configuration MUST change traversal order without code changes.
4. New handlers (e.g. length, spam) MUST be insertable into the chain solely by implementing `ICommentModerationHandler` and registering them — the existing handlers MUST require no modification (Open/Closed).
5. Each handler SHOULD be independently unit-testable in isolation by stubbing its `ICommentAnalyzer` and/or `ModerationOptions` dependencies.
6. The pipeline MUST honor the supplied `CancellationToken` and propagate it to each handler's async work.

### Key Files (planned)

> **PLANNED / TARGET paths — none of these exist yet (greenfield, spec phase only).**

| Planned path | Role |
|---|---|
| `src/ForumGuard.Application/Moderation/ICommentModerationHandler.cs` | CoR handler Strategy contract. |
| `src/ForumGuard.Application/Moderation/CommentModerationContext.cs` | Input context (`Body`, optional `Comment.Id`). |
| `src/ForumGuard.Application/Moderation/ModerationVerdict.cs` | Result `{ Label, Score, IsFlagged, FlaggingHandlerName }`. |
| `src/ForumGuard.Application/Moderation/CommentModerationPipeline.cs` | Chain assembler + traversal + fail-safe wrapper. |
| `src/ForumGuard.Application/Moderation/Handlers/ProfanityPreFilterHandler.cs` | Link 1 — uses `KeywordCommentAnalyzer`. |
| `src/ForumGuard.Application/Moderation/Handlers/MlToxicityHandler.cs` | Link 2 — uses NAS-BERT `ICommentAnalyzer`. |
| `src/ForumGuard.Domain/Interfaces/ICommentAnalyzer.cs` | Strategy consumed by the handlers (defined by the Comment Analysis spec). |
| `src/ForumGuard.Web/Program.cs` | DI registration + configured chain order. |

---

## 3. Validation Rules

### Field-level

| Field | Rule |
|---|---|
| `CommentModerationContext.Body` | MUST be non-null. A null `Body` MUST be treated as empty string by the pipeline (no handler may dereference null). |
| `CommentModerationContext.Body` | An empty / whitespace-only `Body` SHOULD pass all handlers and yield `Clean` (no toxic signal); content-length enforcement is the caller's concern per SDD-FORUM-001. |
| `ModerationVerdict.Score` | MUST be a finite float in the range `[0.0, 1.0]`. |
| `ModerationVerdict.FlaggingHandlerName` | MUST be non-null when `IsFlagged == true`; MUST be null when `IsFlagged == false`. |

### Cross-field

| Rule |
|---|
| `IsFlagged` MUST equal `(Label == ToxicityLabel.Toxic)` for every returned verdict. |
| When `IsFlagged == false`, `Label` MUST be `ToxicityLabel.Clean` and `FlaggingHandlerName` MUST be null. |
| `MlToxicityHandler` MUST flag **iff** `Score ≥ Moderation:ToxicityThreshold`; a score exactly equal to the threshold MUST flag (`≥`, inclusive). |

### State-based

| Rule |
|---|
| The chain MUST stop after the first flagging handler; no handler after a flag may execute. |
| Handler ordering used at runtime MUST be the configured order; an empty configured chain MUST yield the default `Clean` verdict. |
| The pipeline MUST be stateless across invocations (no verdict from one comment may leak into another); concurrent evaluations MUST be independent. |

---

## 4. Error Rules

| Trigger | Error type | Domain mapping | Caller / user-facing outcome |
|---|---|---|---|
| A handler throws during evaluation (e.g. ML scoring failure, model unavailable, profanity-list read failure). | (none surfaced) — handled internally | Caught inside `CommentModerationPipeline`; converted to a **fail-safe flagged** `ModerationVerdict` (`Toxic`). Exception logged with handler name + comment id. | The comment goes to `FlaggedForReview`; the user sees the standard "your comment is awaiting review" outcome (SDD-FORUM-001). No error is shown. |
| `CancellationToken` is cancelled mid-evaluation. | cancellation | `OperationCanceledException` MUST be allowed to propagate (it is **not** converted to a fail-safe flag; a cancelled request has no verdict). | The caller's request is aborted per ASP.NET Core cancellation semantics; nothing is persisted. |
| A handler is registered with a duplicate or unresolved `ICommentAnalyzer` dependency at composition time. | configuration | Surfaces as a DI/startup failure (`InvalidOperationException`) at application start — fail fast, not per request. | Application fails to start; operator sees the DI error. Not a runtime user-facing path. |
| `ModerationOptions.ToxicityThreshold` is out of range `[0.0, 1.0]`. | validation | Rejected by options validation in **SDD-FORUM-023** (not by this pipeline); pipeline assumes a validated threshold. | Application fails to start (options validation); see SDD-FORUM-023. |

> This pipeline returns a `ModerationVerdict` (a `Result`-style value), not exceptions, for the
> normal flag/pass outcomes. Exceptions are confined to composition-time configuration faults and
> request cancellation.

---

## 5. Versioning Notes

- **v1 — Initial specification.** Defines `ICommentModerationHandler`, the `CommentModerationPipeline` Chain of Responsibility (`ProfanityPreFilterHandler → MlToxicityHandler → extensible`), short-circuit-on-flag, pass-through, configurable order, empty-chain default-publish, and fail-safe-on-exception (flag) semantics. Establishes the `ModerationVerdict` result contract and its mapping to `CommentStatus` by the caller (SDD-FORUM-001).

---

## 6. Test Plan

Business tests reference this spec via `[Category("SDD-FORUM-021")]`. Test names follow
`MethodName_Scenario_ExpectedResult`.

### Unit tests

| Test name | Tag |
|---|---|
| `EvaluateAsync_CleanText_TraversesWholeChainAndReturnsClean` | `[Unit]` |
| `EvaluateAsync_CleanText_ReturnsIsFlaggedFalseAndNullFlaggingHandler` | `[Unit]` |
| `EvaluateAsync_ProfanityKeyword_FlaggedByProfanityPreFilterHandler` | `[Unit]` |
| `EvaluateAsync_ProfanityKeyword_DoesNotInvokeMlAnalyzer` | `[Unit]` |
| `EvaluateAsync_ProfanityKeyword_SetsFlaggingHandlerNameToProfanityPreFilter` | `[Unit]` |
| `EvaluateAsync_MlScoreAboveThreshold_FlaggedByMlToxicityHandler` | `[Unit]` |
| `EvaluateAsync_MlScoreEqualToThreshold_FlagsInclusively` | `[Unit]` |
| `EvaluateAsync_MlScoreBelowThreshold_PassesAndReturnsClean` | `[Unit]` |
| `EvaluateAsync_EmptyChain_ReturnsCleanWithZeroScore` | `[Unit]` |
| `EvaluateAsync_EmptyChain_DoesNotThrow` | `[Unit]` |
| `EvaluateAsync_FirstHandlerFlags_DoesNotRunSubsequentHandlers` | `[Unit]` |
| `EvaluateAsync_HandlerThrows_ReturnsFailSafeFlaggedVerdict` | `[Unit]` |
| `EvaluateAsync_HandlerThrows_DoesNotPropagateException` | `[Unit]` |
| `EvaluateAsync_HandlerThrows_LogsHandlerNameAndCommentId` | `[Unit]` |
| `EvaluateAsync_HandlerThrows_NeverReturnsCleanVerdict` | `[Unit]` |
| `EvaluateAsync_NullBody_TreatedAsEmptyAndReturnsClean` | `[Unit]` |
| `EvaluateAsync_ConfiguredOrderReversed_TraversesInConfiguredOrder` | `[Unit]` |
| `EvaluateAsync_NewCustomHandlerRegistered_RunsWithoutModifyingExistingHandlers` | `[Unit]` |
| `EvaluateAsync_CancellationRequested_ThrowsOperationCanceledException` | `[Unit]` |
| `Verdict_FlaggedResult_IsFlaggedEqualsLabelToxic` | `[Unit]` |
| `Verdict_CleanResult_FlaggingHandlerNameIsNull` | `[Unit]` |

### Integration tests

> The pipeline crosses the DI / configuration boundary (handlers resolved from the container,
> chain order bound from configuration, real `KeywordCommentAnalyzer` + `ModerationOptions`).

| Test name | Tag |
|---|---|
| `Pipeline_ResolvedFromContainer_RunsHandlersInConfiguredChainOrder` | `[Integration]` |
| `Pipeline_RealKeywordAnalyzerAndProfanityList_FlagsKnownProfanityBeforeMl` | `[Integration]` |
| `Pipeline_ToxicityThresholdFromConfiguration_AppliedByMlToxicityHandler` | `[Integration]` |
| `Pipeline_AdditionalHandlerRegisteredInDi_ParticipatesInChain` | `[Integration]` |
| `Pipeline_MlAnalyzerFailsToLoadModel_VerdictFailsSafeToFlagged` | `[Integration]` |
