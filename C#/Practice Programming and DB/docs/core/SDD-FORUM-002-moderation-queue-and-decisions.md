# Moderation Queue & Decisions

| Field | Value |
|---|---|
| **Title** | Moderation Queue & Decisions |
| **Spec ID** | SDD-FORUM-002 |
| **Category** | core |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 1. Context & Scope

This spec defines the **moderator review workflow**: how a signed-in **Moderator** views the queue
of comments in status `FlaggedForReview`, and how the Moderator **Approves** or **Rejects** each one.
Every decision is recorded as a `ModerationDecision` audit row (who decided, when, the
`ModerationOutcome`, and an optional reason).

A comment enters this queue only after the moderation pipeline (SDD-FORUM-021) classifies it as
`Toxic` and the submission flow (SDD-FORUM-001) sets its status to `FlaggedForReview`. This spec
picks up at that point and drives the comment to a **terminal** status:

- **Approve** → `ApprovedByModerator` (comment becomes publicly visible; `PublishedAtUtc` set).
- **Reject** → `RejectedByModerator` (comment stays hidden permanently).

### Covered

- The Moderator-facing queue listing of `FlaggedForReview` comments (Razor Page).
- The Approve and Reject use cases, including the state transition and the `ModerationDecision` audit write.
- Authorization: only the `CanModerateComments` policy (Moderator) may act here; the **Administrator MUST be denied** (see **SDD-FORUM-011**).
- Concurrency handling when two moderators act on the same comment.
- The visibility consequence of each outcome (Approve publishes; Reject keeps hidden forever).

### Excluded (covered elsewhere)

- The classification that produces `FlaggedForReview` — see **SDD-FORUM-010** (Comment Entity &
  Lifecycle) and the analysis/pipeline behavior in **SDD-FORUM-021** / **SDD-FORUM-001**.
- Definition of the `CanModerateComments` requirement/handler, role membership, and the
  `IRoleWorkspace` (`ModeratorWorkspace`) composition — see **SDD-FORUM-011** (Roles & Authorization Model).
- Persistence mechanics (Repository + Unit of Work + Specification) — see **SDD-FORUM-022** (Data Access).
- `ModerationOptions` binding — see **SDD-FORUM-023** (Configuration & Options).

### Cross-referenced specs

- **SDD-FORUM-001** — Comment Submission & Auto-Publish Flow (upstream; creates `FlaggedForReview`).
- **SDD-FORUM-010** — Comment Entity & Lifecycle (authoritative transition rules).
- **SDD-FORUM-011** — Roles & Authorization Model (`CanModerateComments`, Administrator exclusion, `ModeratorWorkspace`).
- **SDD-FORUM-021** — Moderation Pipeline (Chain of Responsibility; flags the comments that arrive here).
- **SDD-FORUM-022** — Data Access (Repository / Unit of Work / Specification used to load the queue and persist decisions).

### Key Files (planned — code does NOT exist yet)

> All paths below are **TARGET/PLANNED**. No source exists in this greenfield phase.

| Layer | Planned path | Responsibility |
|---|---|---|
| Application | `src/ForumGuard.Application/Moderation/IModerationService.cs` | Use-case contract: list queue, approve, reject. |
| Application | `src/ForumGuard.Application/Moderation/ModerationService.cs` | Orchestrates state transition + `ModerationDecision` write via Unit of Work. |
| Application | `src/ForumGuard.Application/Moderation/Dtos/QueuedCommentDto.cs` | Read DTO for the queue listing (no EF entity exposed). |
| Application | `src/ForumGuard.Application/Moderation/Dtos/ModerationDecisionRequest.cs` | Input DTO `{ Guid CommentId, ModerationOutcome Decision, string? Reason }`. |
| Domain | `src/ForumGuard.Domain/Specifications/FlaggedForReviewCommentsSpecification.cs` | Specification selecting `Status == FlaggedForReview`. |
| Web | `src/ForumGuard.Web/Pages/Moderation/Queue.cshtml(.cs)` | Razor Page listing the queue (guarded by `CanModerateComments`). |
| Web | `src/ForumGuard.Web/Pages/Moderation/Review.cshtml(.cs)` | Razor Page handlers `OnPostApprove` / `OnPostReject`. |
| Tests | `tests/ForumGuard.Tests/Moderation/ModerationServiceTests.cs` | Unit tests for the use cases. |
| Tests | `tests/ForumGuard.Tests/Moderation/ModerationQueuePageTests.cs` | Integration tests crossing the Razor request boundary. |

---

## 2. Behavior

RFC-2119 keywords (**MUST**, **SHOULD**, **MAY**) are used. Every **MUST**/**SHOULD** below is
independently testable; the Test Plan (§7) provides the corresponding test.

### 2.1 Happy path — view queue and approve

- **B1.** The moderation queue **MUST** list exactly the comments whose `Status` is
  `FlaggedForReview`. Comments in any other status (`PendingAnalysis`, `Published`,
  `ApprovedByModerator`, `RejectedByModerator`) **MUST NOT** appear in the queue.
- **B2.** The queue listing **SHOULD** present, per comment, at least: `Body`, author `DisplayName`,
  `CreatedAtUtc`, `AnalysisLabel`, and `AnalysisScore`, ordered by `CreatedAtUtc` ascending
  (oldest first).
- **B3.** When a Moderator approves a comment that is currently `FlaggedForReview`, the system
  **MUST** transition the comment to `ApprovedByModerator`, set `PublishedAtUtc` to
  `SYSUTCDATETIME()` (the approval instant), and leave the comment publicly visible per the
  visibility invariant (visible IFF `Status` is `Published` OR `ApprovedByModerator`).
- **B4.** On every successful Approve or Reject, the system **MUST** insert exactly one
  `ModerationDecision` row capturing `CommentId`, `ModeratorId` (the acting moderator's id),
  `Decision` (`ModerationOutcome.Approved` or `ModerationOutcome.Rejected`), `DecidedAtUtc`
  (`SYSUTCDATETIME()`), and the optional `Reason`.
- **B5.** The comment status change and the `ModerationDecision` insert **MUST** be committed
  atomically through a single Unit of Work (SDD-FORUM-022); if either fails, neither is persisted.
- **B6.** After a successful decision, the affected comment **MUST** no longer appear in the queue
  (because its status is no longer `FlaggedForReview`).

### 2.2 Happy path — reject

- **B7.** When a Moderator rejects a comment that is currently `FlaggedForReview`, the system
  **MUST** transition the comment to `RejectedByModerator`, **MUST NOT** set `PublishedAtUtc`,
  and the comment **MUST** remain hidden from public listings (it never satisfies the visibility
  invariant). This status is **terminal**.

### 2.3 Edge case — concurrent moderation (double-decision conflict)

- **B8.** If two moderators load the same `FlaggedForReview` comment and both attempt a decision,
  the **first** committed decision **MUST** succeed and the **second MUST** fail with a conflict
  (the comment is no longer `FlaggedForReview`). The system **MUST NOT** apply a second transition
  and **MUST NOT** write a second `ModerationDecision` for that comment in this race.
- **B9.** On the conflicting (second) attempt, the Razor handler **MUST** re-display the queue with
  a user-facing message indicating the comment was already handled by another moderator, and
  **MUST NOT** alter the comment's existing terminal status.

### 2.4 Edge case — Administrator denied (no comment authority)

- **B10.** A signed-in **Administrator MUST** be denied access to the queue page and to the Approve
  and Reject handlers, because the `CanModerateComments` policy excludes Administrators
  (SDD-FORUM-011). The request **MUST** result in an authorization failure (HTTP 403 / Forbid) and
  **MUST NOT** mutate any comment or create a `ModerationDecision`.

### 2.5 Edge case — rejected comment never becomes visible

- **B11.** A comment in `RejectedByModerator` **MUST** never appear in any public thread listing and
  **MUST** never be transitioned back to a visible status by this workflow; `RejectedByModerator`
  is terminal (SDD-FORUM-010).

### 2.6 Additional rules

- **B12.** A decision **MUST** be rejected (conflict) if the target comment is in any status other
  than `FlaggedForReview` (e.g., already `ApprovedByModerator`, `RejectedByModerator`, `Published`,
  or `PendingAnalysis`).
- **B13.** The `Reason` field **MAY** be supplied for either outcome; when omitted it **MUST** be
  stored as `null`. A supplied `Reason` **MUST NOT** exceed 500 characters.
- **B14.** Every queue page request and decision handler **MUST** require an authenticated user who
  satisfies the `CanModerateComments` policy; anonymous requests **MUST** be redirected to login.

---

## 3. Validation Rules

### Field-level

| Field | Rule |
|---|---|
| `ModerationDecisionRequest.CommentId` | Required; **MUST** be a non-empty `Guid`. |
| `ModerationDecisionRequest.Decision` | Required; **MUST** be a defined `ModerationOutcome` value (`Approved` or `Rejected`). |
| `ModerationDecisionRequest.Reason` | Optional; when present **MUST** be ≤ 500 characters (matches `ModerationDecision.Reason` max length). |

### Cross-field / referential

| Rule |
|---|
| The `CommentId` **MUST** resolve to an existing `Comment`; otherwise the request is a *notfound* error. |
| The acting `ModeratorId` **MUST** be the id of the authenticated user resolved from the request principal — never supplied by the client. |

### State-based

| Rule |
|---|
| The target comment **MUST** be in status `FlaggedForReview` at the moment of commit; any other status is a *conflict*. |
| Approve **MUST** set `Status = ApprovedByModerator` and `PublishedAtUtc = SYSUTCDATETIME()`. |
| Reject **MUST** set `Status = RejectedByModerator` and **MUST NOT** set `PublishedAtUtc`. |
| `ApprovedByModerator` and `RejectedByModerator` are terminal — no further transition is permitted by this workflow. |

---

## 4. Error Rules

| # | Trigger | Error type | Domain mapping | Razor user-facing outcome |
|---|---|---|---|---|
| E1 | Caller is not authenticated. | authorization | Challenge (login redirect). | Redirect to the login page. |
| E2 | Caller is authenticated but does not satisfy `CanModerateComments` (includes any **Administrator**). | authorization | `Forbid` (HTTP 403). | "You are not authorized to moderate comments." Forbidden result; no mutation. |
| E3 | `CommentId` does not resolve to an existing `Comment`. | notfound | `Result<T>` failure `NotFound` (or `CommentNotFoundException`). | HTTP 404 / "The comment no longer exists." |
| E4 | Target comment is not in `FlaggedForReview` (e.g., already decided by another moderator — concurrency race). | conflict | `Result<T>` failure `Conflict` (or `InvalidCommentStateException`). | Re-render queue with "This comment was already handled by another moderator." No second `ModerationDecision`. |
| E5 | `Decision` is missing or not a defined `ModerationOutcome` value. | validation | `Result<T>` validation failure (model state invalid). | HTTP 400 / inline validation message; no mutation. |
| E6 | `Reason` exceeds 500 characters. | validation | `Result<T>` validation failure (model state invalid). | HTTP 400 / inline validation message; no mutation. |
| E7 | Persistence fails mid-transaction (e.g., DB optimistic-concurrency or commit error). | conflict | Unit of Work rolls back; `Result<T>` failure `Conflict`. | "Could not save your decision; it may have been handled already. Please retry." No partial write. |

> **Atomicity note:** E4 and E7 both guarantee that on failure neither the comment status change nor
> the `ModerationDecision` row is persisted (B5).

---

## 5. Versioning Notes

- **v1 — Initial specification.** Defines the moderator review queue, the Approve/Reject use cases,
  the `ModerationDecision` audit write, the atomic transition rules, the Administrator denial, and
  the concurrent-decision conflict handling. Aligns with the canonical lifecycle in SDD-FORUM-010
  and the authorization model in SDD-FORUM-011.

---

## 6. Test Plan

Naming convention: `MethodName_Scenario_ExpectedResult`. Business tests reference this spec via
`[Category("SDD-FORUM-002")]`. Tags: `[Unit]` and `[Integration]`. Integration tests exercise the
Razor request boundary, EF Core/MSSQL, and the DI-composed authorization pipeline.

### Unit tests (`[Unit]`)

1. `GetQueue_OnlyFlaggedForReviewComments_ReturnsThoseComments` — verifies B1 (queue contains only `FlaggedForReview`).
2. `GetQueue_MixedStatuses_ExcludesNonFlaggedComments` — verifies B1 (Published/Approved/Rejected/Pending excluded).
3. `GetQueue_MultipleFlagged_OrdersByCreatedAtUtcAscending` — verifies B2 ordering.
4. `Approve_FlaggedComment_TransitionsToApprovedByModeratorAndSetsPublishedAtUtc` — verifies B3.
5. `Approve_FlaggedComment_WritesModerationDecisionWithApprovedOutcome` — verifies B4 for Approve.
6. `Reject_FlaggedComment_TransitionsToRejectedByModeratorAndLeavesPublishedAtUtcNull` — verifies B7.
7. `Reject_FlaggedComment_WritesModerationDecisionWithRejectedOutcome` — verifies B4 for Reject.
8. `Decide_AnyOutcome_RecordsActingModeratorIdAndDecidedAtUtc` — verifies B4 audit fields (who/when).
9. `Approve_CommentNotInFlaggedForReview_ReturnsConflictAndDoesNotTransition` — verifies B12 / E4.
10. `Reject_CommentNotInFlaggedForReview_ReturnsConflictAndDoesNotTransition` — verifies B12 / E4.
11. `Decide_CommentIdNotFound_ReturnsNotFound` — verifies E3.
12. `Decide_MissingOrUndefinedOutcome_ReturnsValidationFailure` — verifies E5.
13. `Decide_ReasonOverFiveHundredChars_ReturnsValidationFailure` — verifies B13 / E6.
14. `Decide_ReasonOmitted_StoresReasonAsNull` — verifies B13 null handling.
15. `Decide_TransitionAndAuditWrite_CommittedAtomically` — verifies B5 (Unit-of-Work atomicity; rollback leaves no partial write).
16. `RejectedComment_RemainsHiddenFromVisibility_NeverSatisfiesVisibilityInvariant` — verifies B11.

### Integration tests (`[Integration]`)

17. `QueuePage_AsModerator_ReturnsOkWithFlaggedComments` — verifies B1/B2/B14 across the Razor boundary with a Moderator principal.
18. `QueuePage_AsAdministrator_ReturnsForbidden` — verifies B10 / E2 (Administrator excluded by `CanModerateComments`).
19. `QueuePage_Anonymous_RedirectsToLogin` — verifies B14 / E1.
20. `PostApprove_AsModerator_PublishesCommentAndPersistsDecision` — verifies B3/B4/B6 end-to-end against EF Core/MSSQL.
21. `PostReject_AsModerator_KeepsCommentHiddenAndPersistsDecision` — verifies B7/B4 end-to-end.
22. `PostApprove_AsAdministrator_ReturnsForbiddenAndDoesNotMutateComment` — verifies B10 / E2 (no mutation, no `ModerationDecision`).
23. `PostDecide_ConcurrentDecisionsOnSameComment_FirstWinsSecondGetsConflict` — verifies B8/B9/E4 (second moderator sees conflict; no second decision row).
24. `PostApprove_AlreadyApprovedComment_ReturnsConflictAndDoesNotWriteSecondDecision` — verifies B12 / E4 / E7.
25. `RejectedComment_NotShownInPublicThreadListing_IsAbsent` — verifies B11 against the public listing query.
