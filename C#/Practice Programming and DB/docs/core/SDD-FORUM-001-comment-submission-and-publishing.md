# Comment Submission & Auto-Publishing

| Field | Value |
|---|---|
| **Title** | Comment Submission & Auto-Publishing |
| **Spec ID** | SDD-FORUM-001 |
| **Category** | core |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 1. Context & Scope

This spec defines the use case in which an authenticated, **active** `User` submits a `Comment`
to an existing `ForumThread`, and the system decides — automatically and synchronously — whether
that comment is **auto-published** or **withheld for moderation**.

The flow is: the comment is persisted with `Status = PendingAnalysis`, then routed through the
moderation pipeline (**SDD-FORUM-021**), which in turn invokes the comment analyzer
(**SDD-FORUM-020**). The pipeline yields a `ToxicityLabel`:

- **Clean** → the comment is published immediately (`Status = Published`, `PublishedAtUtc` set) and
  becomes publicly visible.
- **Toxic** → the comment is withheld (`Status = FlaggedForReview`, not visible) and enters the
  moderator review queue (**SDD-FORUM-002**).

### Covered

- Authorization and account-state gating of the submit action (authenticated `User` whose
  `ApplicationUser.IsActive` is `true`).
- Validation of the submit input (`Body` required and within `Moderation:MaxCommentLength`; target
  `ForumThread` exists).
- Creation of the `Comment` aggregate in `PendingAnalysis` and the synchronous transition to either
  `Published` or `FlaggedForReview`.
- The **fail-safe** behavior when the analysis pipeline throws: the comment MUST be flagged for
  review rather than auto-published.
- Persistence of the analysis outcome (`AnalysisLabel`, `AnalysisScore`, `AnalyzedAtUtc`) on the
  comment.

### Excluded (covered elsewhere)

- The internal mechanics of the Chain-of-Responsibility moderation pipeline and its handlers
  (`ProfanityPreFilter`, `MlToxicity`) — **SDD-FORUM-021**.
- The mechanics of the analyzer Strategy/Adapter/Object-Pool and the `ToxicityThreshold` comparison
  itself — **SDD-FORUM-020**.
- The moderator queue listing and Approve/Reject decisions on `FlaggedForReview` comments —
  **SDD-FORUM-002**.
- Role/policy authorization design (the `User` role membership, `CanModerateComments`, etc.) —
  **SDD-FORUM-011**.
- The authoritative `Comment` lifecycle state machine and visibility invariant — **SDD-FORUM-010**.
- Repository / Unit of Work / Specification persistence contracts — **SDD-FORUM-022**.
- `ModerationOptions` binding (`MaxCommentLength`, `ToxicityThreshold`, `ModelPath`) —
  **SDD-FORUM-023**.

### Key Files (planned — code does not exist yet)

> All paths below are **TARGET** locations for Phase 2 implementation. No source exists in the
> repository at the time of writing.

| Planned path | Responsibility |
|---|---|
| `src/ForumGuard.Web/Pages/Threads/Details.cshtml` + `.cshtml.cs` | Razor Page hosting the comment submit form; `OnPostAsync` handler for the submit POST. |
| `src/ForumGuard.Application/Comments/ICommentSubmissionService.cs` | Application service contract: `SubmitCommentAsync(SubmitCommentRequest, CancellationToken)`. |
| `src/ForumGuard.Application/Comments/CommentSubmissionService.cs` | Orchestrates validation → create `PendingAnalysis` → run pipeline → set terminal/queued status → persist via Unit of Work. |
| `src/ForumGuard.Application/Comments/SubmitCommentRequest.cs` | DTO: `ThreadId`, `Body`, `AuthorId`. |
| `src/ForumGuard.Application/Comments/SubmitCommentResult.cs` | Result DTO conveying outcome (`Published` / `FlaggedForReview`) and validation/error state. |
| `src/ForumGuard.Domain/Entities/Comment.cs` | `Comment` aggregate + lifecycle guard methods (per SDD-FORUM-010). |
| `src/ForumGuard.Application/Moderation/ICommentModerationPipeline.cs` | Pipeline entry point invoked by the submission service (defined by SDD-FORUM-021). |

---

## 2. Behavior

Terminology per RFC 2119. Every **MUST**/**SHOULD** below is independently testable; the matching
test appears in §7.

### 2.1 Happy path — Clean comment auto-publishes

1. An authenticated `User` whose `ApplicationUser.IsActive == true` submits a non-empty `Body` to an
   existing `ForumThread`.
2. The system **MUST** create a `Comment` with `Status = CommentStatus.PendingAnalysis`,
   `AuthorId` = the signed-in user, `ThreadId` = the target thread, and `CreatedAtUtc` =
   `SYSUTCDATETIME()` (server UTC).
3. The system **MUST** run the new comment through the moderation pipeline (SDD-FORUM-021) **before**
   the submit request completes (analysis is synchronous from the user's perspective).
4. The system **MUST** persist the analysis outcome onto the comment: `AnalysisLabel`,
   `AnalysisScore`, and `AnalyzedAtUtc`.
5. When the pipeline returns `ToxicityLabel.Clean`, the system **MUST** set
   `Status = CommentStatus.Published` and set `PublishedAtUtc = SYSUTCDATETIME()`.
6. A `Published` comment **MUST** be publicly visible immediately (visibility invariant per
   SDD-FORUM-010: visible IFF `Status` is `Published` OR `ApprovedByModerator`).
7. The submit handler **SHOULD** report to the user that the comment was published.

### 2.2 Happy path — Toxic comment is withheld

1. Same preconditions as 2.1.
2. When the pipeline returns `ToxicityLabel.Toxic`, the system **MUST** set
   `Status = CommentStatus.FlaggedForReview` and **MUST NOT** set `PublishedAtUtc` (it remains
   `null`).
3. A `FlaggedForReview` comment **MUST NOT** be publicly visible.
4. The comment **MUST** become available to the moderator review queue (SDD-FORUM-002).
5. The submit handler **SHOULD** report to the user that the comment was submitted and is awaiting
   review (without disclosing the toxicity score).

### 2.3 Edge case — Analysis pipeline throws (fail-safe)

1. If the moderation pipeline (SDD-FORUM-021) or the analyzer (SDD-FORUM-020) raises an exception
   during analysis, the system **MUST** treat the comment as unsafe: it **MUST** set
   `Status = CommentStatus.FlaggedForReview` and **MUST NOT** auto-publish.
2. The system **MUST** still persist the comment (it MUST NOT be lost) and **MUST** leave
   `PublishedAtUtc = null`.
3. `AnalysisLabel` and `AnalysisScore` **MAY** be left `null` when no result was produced;
   `AnalyzedAtUtc` **SHOULD** be set to the time the analysis attempt completed/failed.
4. The system **MUST NOT** surface the internal exception detail to the user; it **SHOULD** report
   that the comment is awaiting review.

### 2.4 Edge case — Score exactly at the toxicity threshold

1. When the analyzer's toxic score equals `Moderation:ToxicityThreshold` exactly, the classification
   **MUST** resolve to `ToxicityLabel.Toxic` (the comparison is `toxicScore >= ToxicityThreshold`,
   inclusive — see SDD-FORUM-020), and therefore the comment **MUST** become `FlaggedForReview`, not
   `Published`.
2. This boundary behavior **MUST** be deterministic and independent of submission order.

### 2.5 Edge case — Submission to a non-existent thread

1. If `ThreadId` does not correspond to an existing `ForumThread`, the system **MUST NOT** create any
   `Comment` and **MUST** return a not-found outcome (see §5).

### 2.6 Edge case — Inactive account

1. If the signed-in `ApplicationUser.IsActive == false`, the system **MUST NOT** create a `Comment`
   and **MUST** reject the submission as unauthorized (see §5), regardless of `Body` validity.

---

## 3. Validation Rules

### 3.1 Field-level

| Field | Rule |
|---|---|
| `Body` | **MUST** be present and non-whitespace. **MUST NOT** exceed `Moderation:MaxCommentLength` (bound from `ModerationOptions.MaxCommentLength`; domain hard max for `Comment.Body` is 4000 per SDD-FORUM-010). Leading/trailing whitespace **SHOULD** be trimmed before length evaluation and persistence. |
| `ThreadId` | **MUST** be a non-empty `Guid` referencing an existing `ForumThread`. |
| `AuthorId` | **MUST** equal the signed-in user's `Id`; the submission **MUST NOT** allow authoring on behalf of another user. |

### 3.2 Cross-field

- `MaxCommentLength` from `ModerationOptions` **MUST NOT** be configured above the domain maximum of
  4000; if it is, the effective limit applied to `Body` **MUST** be the lower of the two (the
  persistence layer constrains `Comment.Body` to 4000 — SDD-FORUM-022).

### 3.3 State-based

- The submitter **MUST** be authenticated **AND** `ApplicationUser.IsActive == true` for any
  `Comment` to be created.
- A newly created `Comment` **MUST** start in `CommentStatus.PendingAnalysis`; no other initial
  status is permitted (SDD-FORUM-010).
- The transition out of `PendingAnalysis` **MUST** be exactly one of `Published` or
  `FlaggedForReview`; no `Comment` may remain in `PendingAnalysis` after the submit request completes
  successfully.

---

## 4. Error Rules

| # | Trigger | Type | Domain mapping | Razor user-facing outcome |
|---|---|---|---|---|
| E1 | Request is not authenticated. | authorization | Challenge / `Result` failure (`Unauthorized`). | Redirect to login; no comment created. |
| E2 | Authenticated but `ApplicationUser.IsActive == false`. | authorization | `Result` failure (`Forbidden`); no exception leak. | Friendly message: account is deactivated, contact an administrator. No comment created. |
| E3 | `Body` is null, empty, or whitespace. | validation | `Result` failure with field error on `Body`. | Inline validation error on the submit form; form redisplayed with input preserved. |
| E4 | `Body` exceeds the effective `MaxCommentLength`. | validation | `Result` failure with field error on `Body`. | Inline validation error stating the max length; form redisplayed. |
| E5 | `ThreadId` does not match an existing `ForumThread`. | notfound | `Result` failure (`NotFound`); no `Comment` persisted. | 404 page / "thread not found" message; no comment created. |
| E6 | `AuthorId` does not equal the signed-in user. | authorization | `Result` failure (`Forbidden`). | Friendly authorization error; no comment created. |
| E7 | Moderation pipeline / analyzer throws during analysis. | (handled, not surfaced) | Caught internally; comment persisted as `FlaggedForReview` (fail-safe, §2.3). The exception **SHOULD** be logged. | Friendly message: comment submitted and awaiting review. No internal detail shown. |

> Application services **SHOULD** return a `Result`/`Result<T>` outcome for validation, not-found,
> and authorization failures rather than throwing for control flow; genuine infrastructure faults
> (DB unavailable) **MAY** propagate as exceptions and surface as a generic error page.

---

## 5. Versioning Notes

- **v1 — Initial specification.** Defines authenticated/active submission gating, `PendingAnalysis`
  creation, synchronous routing through the moderation pipeline (SDD-FORUM-021) and analyzer
  (SDD-FORUM-020), Clean→`Published` auto-publish, Toxic→`FlaggedForReview` withholding, the
  fail-safe flag-on-error rule, and the inclusive at-threshold boundary.

---

## 6. Test Plan

Test naming: `MethodName_Scenario_ExpectedResult`. Business tests carry
`[Category("SDD-FORUM-001")]`. Unit tests stub the moderation pipeline / `ICommentAnalyzer`
(the `KeywordCommentAnalyzer` Strategy is used model-free where a real classification is needed).
Integration tests cross the Razor request boundary and the EF Core persistence boundary.

### Unit

- `[Unit]` `SubmitCommentAsync_CleanResult_SetsStatusPublishedAndPublishedAtUtc`
- `[Unit]` `SubmitCommentAsync_CleanResult_PersistsAnalysisLabelScoreAndAnalyzedAtUtc`
- `[Unit]` `SubmitCommentAsync_ToxicResult_SetsStatusFlaggedForReviewAndLeavesPublishedAtUtcNull`
- `[Unit]` `SubmitCommentAsync_NewComment_StartsInPendingAnalysis`
- `[Unit]` `SubmitCommentAsync_PipelineThrows_FlagsForReviewAndDoesNotPublish`
- `[Unit]` `SubmitCommentAsync_PipelineThrows_PersistsCommentAndLeavesPublishedAtUtcNull`
- `[Unit]` `SubmitCommentAsync_ScoreExactlyAtThreshold_ClassifiesToxicAndFlagsForReview`
- `[Unit]` `SubmitCommentAsync_EmptyBody_ReturnsValidationFailureAndCreatesNoComment`
- `[Unit]` `SubmitCommentAsync_WhitespaceBody_ReturnsValidationFailureAndCreatesNoComment`
- `[Unit]` `SubmitCommentAsync_BodyExceedsMaxCommentLength_ReturnsValidationFailure`
- `[Unit]` `SubmitCommentAsync_BodyTrimmedBeforeLengthCheck_AcceptsBodyWithinLimit`
- `[Unit]` `SubmitCommentAsync_InactiveAccount_ReturnsForbiddenAndCreatesNoComment`
- `[Unit]` `SubmitCommentAsync_AuthorIdNotSignedInUser_ReturnsForbidden`
- `[Unit]` `SubmitCommentAsync_NonExistentThread_ReturnsNotFoundAndCreatesNoComment`

### Integration

- `[Integration]` `OnPostAsync_UnauthenticatedUser_ChallengesAndCreatesNoComment`
- `[Integration]` `OnPostAsync_ActiveUserCleanComment_PersistsPublishedAndIsImmediatelyVisible`
- `[Integration]` `OnPostAsync_ActiveUserToxicComment_PersistsFlaggedForReviewAndNotVisible`
- `[Integration]` `OnPostAsync_InactiveUser_ReturnsForbiddenAndCreatesNoComment`
- `[Integration]` `OnPostAsync_NonExistentThread_ReturnsNotFound`
- `[Integration]` `OnPostAsync_BodyExceedsMaxLength_RedisplaysFormWithValidationError`
- `[Integration]` `SubmitFlow_PipelineFailure_PersistsCommentAsFlaggedForReview`
- `[Integration]` `SubmitFlow_PublishedComment_AppearsInThreadDetailsForOtherUsers`
- `[Integration]` `SubmitFlow_FlaggedComment_DoesNotAppearInThreadDetailsForOtherUsers`
