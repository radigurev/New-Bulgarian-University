# SDD-FORUM-004 — Moderator Role Management

| Field | Value |
|---|---|
| **Title** | Moderator Role Management |
| **Spec ID** | SDD-FORUM-004 |
| **Category** | core |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 1. Context & Scope

This spec defines how an **Administrator** grants and revokes the `"Moderator"` Identity role for a
target `ApplicationUser`. It is the role-administration half of the Administrator's duties; the
account-activation half (`IsActive` toggling) is governed elsewhere.

### Covered

- An Administrator (authorized by the `CanManageModerators` policy) **granting** the `"Moderator"`
  role to a target `ApplicationUser`.
- An Administrator **revoking** the `"Moderator"` role from a target `ApplicationUser`.
- Authorization enforcement: only holders of `CanManageModerators` (Administrators) may perform
  grant/revoke; everyone else is denied.
- Validation that the target user exists and is `IsActive` before a grant.
- Idempotency / redundancy rules for grant of an already-Moderator user and revoke of a
  non-Moderator user.
- Role membership changes performed through ASP.NET Core Identity role APIs
  (`UserManager<ApplicationUser>` / `RoleManager`), executed inside the data-access boundary.

### Excluded

- The definition of the `CanManageModerators`, `CanModerateComments`, and `CanManageAccounts`
  custom authorization requirements/handlers and the `IRoleWorkspace` Strategy — see
  **SDD-FORUM-011** (Roles & Authorization Model).
  > The definition of authorization requirements is in **SDD-FORUM-011** (Roles & Authorization
  > Model, domain spec). This spec (SDD-FORUM-004) **consumes** the `CanManageModerators` policy
  > defined there and does not redefine it.
- Activating/deactivating user profiles (`IsActive`) via the `CanManageAccounts` policy — the
  account-management use case is in **SDD-FORUM-003** (Registration, Login & Account Activation);
  the policy itself is defined in **SDD-FORUM-011**.
- Comment moderation behavior (Approve / Reject) — see **SDD-FORUM-002** (Moderation Queue & Decisions).
- The domain model and the `CommentStatus` lifecycle — see **SDD-FORUM-010** (Comment Entity & Lifecycle).
- Seeding of the initial Administrator account and the three role string constants at startup — see
  the data-access / configuration specs (**SDD-FORUM-022**, **SDD-FORUM-023**); this spec assumes the
  `"User"`, `"Moderator"`, and `"Administrator"` roles already exist.

### Related specs

- **SDD-FORUM-011** — Roles & Authorization Model (Policies + `IRoleWorkspace`): defines
  `CanManageModerators`, `CanManageAccounts`, `CanModerateComments`, and the role workspaces.
- **SDD-FORUM-010** — Comment Entity & Lifecycle (canonical entities; `ApplicationUser` extension fields).
- **SDD-FORUM-002** — Moderation Queue & Decisions (who may act once they hold `"Moderator"`).
- **SDD-FORUM-003** — Registration, Login & Account Activation (the Administrator's account-activation duty).
- **SDD-FORUM-022** — Data Access (Unit of Work boundary used to commit membership changes).

### Key Files (planned — code does not exist yet)

> These are **target** paths for Phase 2 (implementator). None exist in the greenfield repo today.

| Planned path | Responsibility |
|---|---|
| `src/ForumGuard.Application/Services/IModeratorRoleService.cs` | Use-case interface: `GrantModeratorAsync` / `RevokeModeratorAsync`. |
| `src/ForumGuard.Application/Services/ModeratorRoleService.cs` | Orchestrates Identity role APIs + Unit of Work; returns `Result`. |
| `src/ForumGuard.Application/Dtos/ModeratorRoleChangeResult.cs` | Outcome DTO (`Granted` / `Revoked` / `NoOp` + message). |
| `src/ForumGuard.Web/Pages/Admin/Moderators/Index.cshtml(.cs)` | Admin Razor Page listing users and exposing grant/revoke handlers. |
| `src/ForumGuard.Web/Authorization/CanManageModeratorsRequirement.cs` | Custom `IAuthorizationRequirement` (defined under SDD-FORUM-011, consumed here). |
| `src/ForumGuard.Infrastructure/Identity/` | `UserManager<ApplicationUser>` / `RoleManager` registration. |
| `tests/ForumGuard.Tests/` | NUnit unit + integration tests for this spec. |

---

## 2. Behavior

The canonical role string constants are `"User"`, `"Moderator"`, and `"Administrator"`. The use-case
entry points are `GrantModeratorAsync(targetUserId)` and `RevokeModeratorAsync(targetUserId)` exposed
by `IModeratorRoleService` and invoked from the Admin Razor Page handlers.

### 2.1 Happy path — grant the Moderator role

1. An authenticated Administrator opens the moderator-management page and selects an active target
   `ApplicationUser` who is **not** currently a `"Moderator"`, then submits a grant request.
2. The request handler **MUST** require the `CanManageModerators` policy before any work is done.
3. `GrantModeratorAsync` **MUST** load the target `ApplicationUser` by id and **MUST** fail with a
   not-found error if no such user exists.
4. The service **MUST** verify the target user's `IsActive` is `true`; an inactive target **MUST NOT**
   be granted the role.
5. If the target is not already in the `"Moderator"` role, the service **MUST** add the `"Moderator"`
   role via the ASP.NET Core Identity role API (`UserManager<ApplicationUser>.AddToRoleAsync`).
6. The change **MUST** be committed through the Unit of Work boundary (SDD-FORUM-022); on success the
   service **MUST** return a successful `ModeratorRoleChangeResult` with outcome `Granted`.
7. After a successful grant, the target user **MUST** thereafter satisfy `CanModerateComments`
   (i.e., be able to access the moderator queue per SDD-FORUM-002) on their next authenticated request.

### 2.2 Happy path — revoke the Moderator role

1. An authenticated Administrator selects a target `ApplicationUser` who **is** currently a
   `"Moderator"` and submits a revoke request.
2. The handler **MUST** require the `CanManageModerators` policy.
3. `RevokeModeratorAsync` **MUST** load the target user and **MUST** fail with a not-found error if the
   user does not exist.
4. If the target is in the `"Moderator"` role, the service **MUST** remove it via
   `UserManager<ApplicationUser>.RemoveFromRoleAsync` and commit through the Unit of Work, returning a
   successful result with outcome `Revoked`.
5. After a successful revoke, the target user **MUST NOT** satisfy `CanModerateComments` on subsequent
   requests; previously recorded `ModerationDecision` rows **MUST** remain intact (history is not deleted).

### 2.3 Edge case — granting Moderator to an already-Moderator user (redundant grant)

1. When the target already holds the `"Moderator"` role, `GrantModeratorAsync` **MUST NOT** call
   `AddToRoleAsync` again and **MUST NOT** throw.
2. The operation **MUST** be treated as an idempotent **no-op**: the service **MUST** return a
   successful `ModeratorRoleChangeResult` with outcome `NoOp` and a message indicating the user is
   already a Moderator.
3. The membership set **MUST** be unchanged (the user appears in the `"Moderator"` role exactly once).

### 2.4 Edge case — revoking Moderator from a non-Moderator user (redundant revoke)

1. When the target does **not** hold the `"Moderator"` role, `RevokeModeratorAsync` **MUST NOT** call
   `RemoveFromRoleAsync` and **MUST NOT** throw an unexpected exception.
2. The operation **MUST** be treated as an idempotent **no-op**: the service **MUST** return a
   successful `ModeratorRoleChangeResult` with outcome `NoOp` and a message indicating the user was not
   a Moderator. The system **MUST NOT** report this as a hard error to the Administrator.

### 2.5 Edge case — Administrator excluded from comment moderation (separation of duties)

1. Granting or revoking the `"Moderator"` role **MUST NOT** change the acting Administrator's own
   authority over comments. An Administrator **MUST** remain excluded from `CanModerateComments`
   (per SDD-FORUM-011) regardless of any role change they perform.
2. If a single `ApplicationUser` holds **both** `"Administrator"` and `"Moderator"` roles (a permitted
   combination), that user **SHOULD** satisfy `CanModerateComments` **only by virtue of the
   `"Moderator"` role**, never by virtue of `"Administrator"`. The `CanManageModerators` and
   `CanModerateComments` policies remain evaluated independently.
3. An Administrator performing role management **MUST NOT** be required to also be a Moderator;
   the two policies are independent and **MUST** be checked independently.

### 2.6 Edge case — unauthorized caller

1. A request to grant or revoke the `"Moderator"` role from a caller who does **not** satisfy
   `CanManageModerators` (anonymous, `"User"`, or `"Moderator"`-only) **MUST** be denied before any
   Identity role change is attempted.
2. The denial **MUST NOT** disclose whether the target user exists (authorization is checked before
   the not-found lookup outcome is surfaced).

### 2.7 Self-management guard

1. The system **SHOULD** allow an Administrator to grant/revoke `"Moderator"` on any user, including
   themselves, subject to 2.5.
2. The system **MUST NOT** allow the grant/revoke flow to remove the `"Administrator"` role; this spec
   manages the `"Moderator"` role only and **MUST** leave `"Administrator"` membership untouched.

---

## 3. Validation Rules

### Field-level

| Field | Rule |
|---|---|
| `targetUserId` (Guid) | **MUST** be a non-empty `Guid` and **MUST** resolve to an existing `ApplicationUser`; otherwise → not-found error. |
| Role name | **MUST** be exactly the constant `"Moderator"`. The flow **MUST NOT** accept an arbitrary role name parameter for this use case. |

### Cross-field

| Rule |
|---|
| A grant **MUST** require the target `ApplicationUser.IsActive == true`. A revoke **MAY** proceed regardless of `IsActive` (so an Administrator can clean up roles on a deactivated account). |
| The role change **MUST** target a different user **or** the acting Administrator per 2.7; in all cases the `"Administrator"` role membership of any user **MUST** be left unchanged. |

### State-based

| State | Rule |
|---|---|
| Target already in `"Moderator"` | Grant → idempotent `NoOp` (2.3); Revoke → proceeds to remove. |
| Target not in `"Moderator"` | Grant → proceeds to add; Revoke → idempotent `NoOp` (2.4). |
| Target inactive (`IsActive == false`) | Grant → blocked with a validation error; Revoke → permitted. |
| Caller lacks `CanManageModerators` | Both operations → authorization failure, evaluated first (2.6). |

---

## 4. Error Rules

> Application services return `Result` / `ModeratorRoleChangeResult` (no exceptions for expected
> outcomes). Razor Page handlers translate these into page state. Authorization is enforced by the
> framework via the `CanManageModerators` policy (SDD-FORUM-011) before the handler body executes.

| # | Trigger | Type | Domain mapping | Razor user-facing outcome |
|---|---|---|---|---|
| E1 | Caller does not satisfy `CanManageModerators`. | authorization | Policy failure (no service call made). | HTTP 403 / redirect to Access Denied; no page disclosing the target user. |
| E2 | `targetUserId` does not resolve to an `ApplicationUser`. | notfound | `Result.Fail` with `NotFound` reason (no exception). | Page reloads with a "User not found" error message; no membership change. |
| E3 | Grant requested for a target whose `IsActive == false`. | validation | `Result.Fail` with `Validation` reason. | Inline validation message: "Cannot grant Moderator to an inactive account." |
| E4 | Grant requested for a target already in `"Moderator"`. | conflict (handled as idempotent no-op) | `Result.Ok` with outcome `NoOp` — **MUST NOT** throw. | Informational notice: "User is already a Moderator." No error banner. |
| E5 | Revoke requested for a target not in `"Moderator"`. | conflict (handled as idempotent no-op) | `Result.Ok` with outcome `NoOp` — **MUST NOT** throw. | Informational notice: "User was not a Moderator." No error banner. |
| E6 | Underlying Identity API returns an `IdentityResult` with errors, or the Unit of Work commit fails. | conflict / infrastructure | `Result.Fail` carrying the Identity/persistence error reasons; the transaction **MUST** be rolled back so no partial membership change persists. | Error banner: "Could not update the Moderator role. Please try again."; no membership change committed. |
| E7 | Attempt to mutate `"Administrator"` membership through this flow. | validation | `Result.Fail` with `Validation` reason — out of scope for this use case. | Operation rejected; this flow only manages `"Moderator"`. |

---

## 5. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Defines Administrator grant/revoke of the `"Moderator"`
  Identity role: authorization via `CanManageModerators`, target existence + `IsActive` validation,
  idempotent no-op semantics for redundant grant/revoke, separation-of-duties guarantee that the
  Administrator stays excluded from `CanModerateComments`, and preservation of `"Administrator"`
  membership and `ModerationDecision` history.

---

## 6. Test Plan

> Convention: `MethodName_Scenario_ExpectedResult`. Business tests carry
> `[Category("SDD-FORUM-004")]`. Unit tests stub `UserManager<ApplicationUser>` / repositories;
> integration tests exercise the Admin Razor Page through the request/authorization boundary against
> a real EF Core + Identity store.

### Unit — `ModeratorRoleService`

- `GrantModeratorAsync_ActiveUserNotModerator_AddsModeratorRoleAndReturnsGranted` `[Unit]`
- `GrantModeratorAsync_TargetUserNotFound_ReturnsNotFoundFailure` `[Unit]`
- `GrantModeratorAsync_InactiveTarget_ReturnsValidationFailureAndDoesNotAddRole` `[Unit]`
- `GrantModeratorAsync_AlreadyModerator_ReturnsNoOpAndDoesNotCallAddToRole` `[Unit]`
- `GrantModeratorAsync_IdentityResultFails_ReturnsFailureAndDoesNotCommit` `[Unit]`
- `GrantModeratorAsync_OnlyMutatesModeratorRole_LeavesAdministratorMembershipUnchanged` `[Unit]`
- `RevokeModeratorAsync_ExistingModerator_RemovesRoleAndReturnsRevoked` `[Unit]`
- `RevokeModeratorAsync_TargetUserNotFound_ReturnsNotFoundFailure` `[Unit]`
- `RevokeModeratorAsync_NotAModerator_ReturnsNoOpAndDoesNotCallRemoveFromRole` `[Unit]`
- `RevokeModeratorAsync_InactiveTarget_StillRevokesSuccessfully` `[Unit]`
- `RevokeModeratorAsync_PreservesExistingModerationDecisionHistory` `[Unit]`
- `GrantModeratorAsync_SuccessfulChange_CommitsThroughUnitOfWorkExactlyOnce` `[Unit]`

### Integration — Admin Razor Page + authorization + Identity store

- `GrantModeratorHandler_AdministratorGrantsActiveUser_PersistsModeratorRole` `[Integration]`
- `GrantModeratorHandler_NonAdministratorCaller_ReturnsForbiddenAndMakesNoChange` `[Integration]`
- `GrantModeratorHandler_AnonymousCaller_ChallengesBeforeNotFoundDisclosure` `[Integration]`
- `RevokeModeratorHandler_AdministratorRevokesModerator_RemovesRoleFromStore` `[Integration]`
- `GrantModeratorHandler_AdministratorWhoIsAlsoModerator_StillExcludedFromCanModerateComments` `[Integration]`
- `RevokeModeratorHandler_NonexistentTargetUser_ReturnsNotFoundWithoutChange` `[Integration]`
- `GrantedModerator_OnNextRequest_SatisfiesCanModerateCommentsPolicy` `[Integration]`
