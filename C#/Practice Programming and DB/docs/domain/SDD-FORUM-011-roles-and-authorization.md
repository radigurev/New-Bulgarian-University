# Roles & Authorization Model

| Field | Value |
|---|---|
| **Title** | Roles & Authorization Model |
| **Spec ID** | SDD-FORUM-011 |
| **Category** | domain |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 1. Context & Scope

This spec defines the **authorization model** for ForumGuard: the three Identity roles, the three
custom authorization requirements/handlers that gate sensitive actions, and the `IRoleWorkspace`
Strategy that composes each role's dashboard, menu, and available actions. It is the authoritative
source for **who is allowed to do what** and **how that decision is made in code**.

The authorization logic specified here is pure decision logic (claims/roles in → grant/deny out)
and is therefore classified as a **domain** concern, even though it is physically realized in the
security layer of `ForumGuard.Web`. This spec is the single source of truth that the request-boundary
behavior specs (`SDD-FORUM-002`, `SDD-FORUM-003`, `SDD-FORUM-004`) consume:
`SDD-FORUM-002` applies `CanModerateComments`, `SDD-FORUM-003` applies `CanManageAccounts`, and
`SDD-FORUM-004` applies `CanManageModerators`.

### Covered

- The three Identity roles and their string constants: `"User"`, `"Moderator"`, `"Administrator"`.
- The three custom authorization requirements + handlers (Strategy in the security layer):
  - `CanModerateComments` — granted to `"Moderator"` **only**; `"Administrator"` is explicitly excluded.
  - `CanManageModerators` — granted to `"Administrator"` **only**.
  - `CanManageAccounts` — granted to `"Administrator"` **only**.
- Named authorization **policies** that bind each requirement.
- The `IRoleWorkspace` Strategy (`UserWorkspace` / `ModeratorWorkspace` / `AdministratorWorkspace`)
  and the `IRoleWorkspaceResolver` keyed on the signed-in user's role.
- Decision behavior for edge cases: anonymous users and users holding multiple roles.

### Excluded (covered elsewhere)

- The HTTP request-boundary application of these policies on Razor Page handlers (the `[Authorize(Policy = ...)]`
  wiring, redirects, status codes) — applied by `SDD-FORUM-002` (queue), `SDD-FORUM-003` (account
  activation), and `SDD-FORUM-004` (Moderator role management).
- The moderator review queue and Approve/Reject use cases — see `SDD-FORUM-002`.
- The moderation Chain-of-Responsibility pipeline — see `SDD-FORUM-021`.
- Comment lifecycle and the visibility invariant — see `SDD-FORUM-010` (Comment Entity & Lifecycle).
- Account activation/deactivation (`IsActive`) and Moderator role grant/revoke **use cases** — gated by
  `CanManageAccounts` here (workflow in `SDD-FORUM-003`) and `CanManageModerators` here (workflow in
  `SDD-FORUM-004`).
- Identity storage, password rules, and login — ASP.NET Core Identity defaults; out of scope.

### Related specs

- `SDD-FORUM-002` — Moderation Queue & Decisions: gated by `CanModerateComments`.
- `SDD-FORUM-003` — Registration, Login & Account Activation: gated by `CanManageAccounts`.
- `SDD-FORUM-004` — Moderator Role Management: gated by `CanManageModerators`.
- `SDD-FORUM-010` — Comment Entity & Lifecycle: the comment lifecycle/visibility invariant.
- `SDD-FORUM-021` — Moderation Pipeline (Chain of Responsibility): consumes `CanModerateComments` indirectly via the queue.

### Key Files (planned — code does not exist yet)

> All paths below are **TARGET** locations. No source code exists yet (greenfield). These describe
> where the implementator (Phase 2) MUST place the artifacts this spec defines.

| Artifact | Planned path |
|---|---|
| Role name constants | `src/ForumGuard.Domain/Authorization/ForumRoles.cs` |
| Policy name constants | `src/ForumGuard.Domain/Authorization/ForumPolicies.cs` |
| `CanModerateComments` requirement | `src/ForumGuard.Web/Authorization/Requirements/CanModerateCommentsRequirement.cs` |
| `CanModerateComments` handler | `src/ForumGuard.Web/Authorization/Handlers/CanModerateCommentsHandler.cs` |
| `CanManageModerators` requirement | `src/ForumGuard.Web/Authorization/Requirements/CanManageModeratorsRequirement.cs` |
| `CanManageModerators` handler | `src/ForumGuard.Web/Authorization/Handlers/CanManageModeratorsHandler.cs` |
| `CanManageAccounts` requirement | `src/ForumGuard.Web/Authorization/Requirements/CanManageAccountsRequirement.cs` |
| `CanManageAccounts` handler | `src/ForumGuard.Web/Authorization/Handlers/CanManageAccountsHandler.cs` |
| `IRoleWorkspace` Strategy interface | `src/ForumGuard.Application/Workspaces/IRoleWorkspace.cs` |
| `UserWorkspace` | `src/ForumGuard.Application/Workspaces/UserWorkspace.cs` |
| `ModeratorWorkspace` | `src/ForumGuard.Application/Workspaces/ModeratorWorkspace.cs` |
| `AdministratorWorkspace` | `src/ForumGuard.Application/Workspaces/AdministratorWorkspace.cs` |
| `IRoleWorkspaceResolver` + resolver | `src/ForumGuard.Application/Workspaces/RoleWorkspaceResolver.cs` |
| Policy + handler + workspace DI registration | `src/ForumGuard.Web/Program.cs` |

---

## 2. Behavior

Requirement keywords follow RFC 2119 (**MUST**, **SHOULD**, **MAY**). Every MUST/SHOULD below is
independently testable.

### 2.1 Roles (happy path)

- **B-1.** The system MUST define exactly three Identity roles, referenced through string constants:
  `"User"`, `"Moderator"`, and `"Administrator"`. These constants MUST be the single source for role
  names; no handler, policy, or workspace MAY hard-code a role literal independently.
- **B-2.** Every successfully registered account MUST be assigned the `"User"` role. `"Moderator"` and
  `"Administrator"` are additive roles granted on top of `"User"`.
- **B-3.** A user MAY hold more than one role simultaneously (e.g., `"User"` + `"Moderator"`). Authorization
  decisions MUST evaluate the union of the user's roles.

### 2.2 Custom authorization requirements + handlers (Strategy)

Each policy is realized as a custom `IAuthorizationRequirement` paired with an `AuthorizationHandler<T>`.
A handler decides over the supplied `AuthorizationHandlerContext` (which carries the user's `ClaimsPrincipal`).

#### CanModerateComments

- **B-4.** The `CanModerateComments` handler MUST call `context.Succeed(requirement)` when the principal is
  in role `"Moderator"`.
- **B-5.** The `CanModerateComments` handler MUST NOT succeed for a principal in role `"Administrator"`
  unless that same principal is also in role `"Moderator"`. The `"Administrator"` role alone MUST be denied
  (the Administrator has no authority over comments).
- **B-6.** The `CanModerateComments` handler MUST NOT succeed for a principal in role `"User"` only.
- **B-7.** For a principal holding both `"Moderator"` and `"Administrator"`, the `CanModerateComments`
  handler MUST succeed (the presence of `"Moderator"` is sufficient; `"Administrator"` neither grants nor
  revokes it).

#### CanManageModerators

- **B-8.** The `CanManageModerators` handler MUST succeed when the principal is in role `"Administrator"`.
- **B-9.** The `CanManageModerators` handler MUST NOT succeed for a principal in role `"Moderator"` only
  or `"User"` only.

#### CanManageAccounts

- **B-10.** The `CanManageAccounts` handler MUST succeed when the principal is in role `"Administrator"`.
- **B-11.** The `CanManageAccounts` handler MUST NOT succeed for a principal in role `"Moderator"` only
  or `"User"` only.

#### Common handler behavior

- **B-12.** A handler MUST express a denial by **not** calling `context.Succeed(requirement)` (i.e., it
  leaves the requirement unmet). A handler MUST NOT throw to express a routine denial.
- **B-13.** Each policy MUST be registered under a stable policy name constant (e.g., `ForumPolicies.CanModerateComments`)
  that binds exactly its corresponding requirement; the three policy names MUST be distinct.
- **B-14.** Each handler MUST evaluate roles via the principal's role claims only and MUST NOT query the
  database, so the decision is deterministic and unit-testable in isolation.

### 2.3 IRoleWorkspace Strategy & resolver

- **B-15.** `IRoleWorkspace` MUST expose, at minimum: a `RoleName` identifier, a `DashboardTitle`,
  an ordered set of menu items, and the set of `AvailableActions` for that role. Each concrete workspace
  is a Strategy implementation.
- **B-16.** `UserWorkspace` MUST advertise actions limited to creating threads and writing comments. It
  MUST NOT advertise moderation, moderator-management, or account-management actions.
- **B-17.** `ModeratorWorkspace` MUST advertise the moderation queue / Approve / Reject actions (those gated
  by `CanModerateComments`). It MUST NOT advertise moderator-management or account-management actions.
- **B-18.** `AdministratorWorkspace` MUST advertise the manage-moderators and manage-accounts actions (those
  gated by `CanManageModerators` and `CanManageAccounts`). It MUST NOT advertise any comment-moderation
  action (consistent with B-5).
- **B-19.** `IRoleWorkspaceResolver.Resolve(principal)` MUST return the `AdministratorWorkspace` when the
  principal is in role `"Administrator"`, otherwise the `ModeratorWorkspace` when in role `"Moderator"`,
  otherwise the `UserWorkspace`. Resolution precedence MUST be Administrator > Moderator > User.
- **B-20.** The set of `AvailableActions` exposed by each workspace MUST be consistent with the policies in
  §2.2 — i.e., a workspace MUST NOT advertise an action whose gating policy would deny that workspace's role.
  (This is the testable link between menu composition and authorization.)

### 2.4 Edge cases

- **B-21. (Edge: anonymous user)** When the principal is unauthenticated (anonymous), every handler in §2.2
  MUST leave its requirement unmet (no `Succeed`). `IRoleWorkspaceResolver.Resolve` MUST treat an anonymous
  or null principal as the `UserWorkspace` baseline and MUST NOT throw.
- **B-22. (Edge: multiple roles)** A principal in `"Moderator"` + `"Administrator"` MUST be granted
  `CanModerateComments` (via `"Moderator"`), `CanManageModerators`, and `CanManageAccounts` (via `"Administrator"`);
  the resolver MUST return `AdministratorWorkspace` for that principal (precedence per B-19).
- **B-23. (Edge: deactivated account)** Role-based authorization decisions in §2.2 are evaluated from the
  principal's role claims and do not by themselves consult `IsActive`; enforcement of `IsActive == false`
  (sign-out / access denial) is the responsibility of the request-boundary spec `SDD-FORUM-003`. This spec
  therefore makes no claim that an inactive user is auto-denied by these handlers.

---

## 3. Validation Rules

### Field-level

- **V-1.** Role name constants MUST be exactly `"User"`, `"Moderator"`, `"Administrator"` (case-sensitive,
  matching Identity seed data).
- **V-2.** Policy name constants MUST be non-empty, distinct, and stable across the application lifetime.
- **V-3.** A concrete `IRoleWorkspace` MUST have a non-null `RoleName` equal to one of the three role
  constants and a non-empty `AvailableActions` set (an empty workspace is invalid).

### Cross-field

- **V-4.** Each named policy MUST bind exactly one requirement type; a policy MUST NOT bind a requirement
  belonging to a different policy (e.g., `CanManageAccounts` MUST NOT be satisfied by the
  `CanModerateComments` handler).
- **V-5.** A workspace's `AvailableActions` MUST be a subset of the actions permitted to its `RoleName`
  under §2.2 (cross-checks B-20).

### State-based

- **V-6.** Authorization handlers MUST be stateless and idempotent: evaluating the same principal twice
  MUST yield the same decision.
- **V-7.** Resolver output MUST depend only on the principal's roles at resolution time and MUST be stable
  for an unchanged principal (no hidden mutation between calls).

---

## 4. Error Rules

These handlers and the resolver are pure decision logic; routine "deny" outcomes are **not** errors (B-12).
Errors below cover misconfiguration and the request-boundary surface that consumes this spec.

| # | Trigger | Type | Domain mapping | Razor user-facing outcome (via the consuming specs `SDD-FORUM-002` / `SDD-FORUM-003` / `SDD-FORUM-004`) |
|---|---|---|---|---|
| E-1 | A handler is asked to evaluate a principal lacking the required role. | authorization | Requirement left unmet (no `Succeed`); no exception. | Handler at the request boundary returns `403 Forbidden`; user redirected to the Access Denied page. |
| E-2 | An anonymous (unauthenticated) principal hits a policy-gated action. | authorization | Requirement left unmet (no `Succeed`); resolver returns `UserWorkspace`. | Redirect to the login page (challenge), per ASP.NET Core defaults. |
| E-3 | A requested policy name is not registered in the authorization options. | conflict (misconfiguration) | `InvalidOperationException` raised by the ASP.NET Core authorization middleware at startup/first use. | Surfaced as a server error; MUST be caught in tests, never reach an end user in production config. |
| E-4 | `IRoleWorkspaceResolver.Resolve` receives a null principal. | validation | MUST NOT throw; returns `UserWorkspace` baseline (B-21). | Anonymous menu rendered; no error shown. |
| E-5 | A workspace advertises an action its role is denied (V-5 / B-20 violated). | conflict (spec violation) | Detected by tests as a defect; not a runtime exception path. | N/A — must be prevented before release. |

---

## 5. Versioning Notes

- **v1 — Initial specification.** Defines the three Identity roles, the three custom authorization
  requirements/handlers (`CanModerateComments`, `CanManageModerators`, `CanManageAccounts`) with the
  Administrator explicitly excluded from comment moderation, and the `IRoleWorkspace` Strategy plus
  role-keyed resolver. Greenfield, authoritative.

---

## 6. Test Plan

Tests reference this spec via `[Category("SDD-FORUM-011")]`. Naming convention:
`MethodName_Scenario_ExpectedResult`. Handlers and the resolver are pure logic, so coverage is
predominantly `[Unit]`; two `[Integration]` tests exercise the handlers through the
`IAuthorizationService` / policy pipeline as configured in `Program.cs`.

### Unit tests — CanModerateComments

- `HandleRequirement_ModeratorPrincipal_SucceedsRequirement` [Unit]
- `HandleRequirement_AdministratorOnlyPrincipal_DoesNotSucceed` [Unit]
- `HandleRequirement_UserOnlyPrincipal_DoesNotSucceed` [Unit]
- `HandleRequirement_ModeratorAndAdministratorPrincipal_SucceedsRequirement` [Unit]
- `HandleRequirement_AnonymousPrincipal_DoesNotSucceed` [Unit]

### Unit tests — CanManageModerators

- `HandleRequirement_AdministratorPrincipal_SucceedsRequirement` [Unit]
- `HandleRequirement_ModeratorOnlyPrincipal_DoesNotSucceed` [Unit]
- `HandleRequirement_UserOnlyPrincipal_DoesNotSucceed` [Unit]

### Unit tests — CanManageAccounts

- `HandleRequirement_AdministratorPrincipal_SucceedsAccountRequirement` [Unit]
- `HandleRequirement_ModeratorOnlyPrincipal_DoesNotSucceedAccountRequirement` [Unit]
- `HandleRequirement_AnonymousPrincipal_DoesNotSucceedAccountRequirement` [Unit]

### Unit tests — IRoleWorkspace & resolver

- `Resolve_AdministratorPrincipal_ReturnsAdministratorWorkspace` [Unit]
- `Resolve_ModeratorPrincipal_ReturnsModeratorWorkspace` [Unit]
- `Resolve_UserPrincipal_ReturnsUserWorkspace` [Unit]
- `Resolve_ModeratorAndAdministratorPrincipal_ReturnsAdministratorWorkspace` [Unit]
- `Resolve_NullPrincipal_ReturnsUserWorkspace` [Unit]
- `Resolve_AnonymousPrincipal_ReturnsUserWorkspace` [Unit]
- `AvailableActions_AdministratorWorkspace_ExcludesCommentModeration` [Unit]
- `AvailableActions_ModeratorWorkspace_ExcludesAccountAndModeratorManagement` [Unit]
- `AvailableActions_UserWorkspace_ContainsOnlyAuthoringActions` [Unit]
- `AvailableActions_EachWorkspace_IsSubsetOfRolePermittedActions` [Unit]

### Unit tests — policy/constant configuration

- `RoleConstants_AreExactlyExpectedValues_MatchIdentitySeed` [Unit]
- `PolicyNames_AreDistinctAndNonEmpty_ReturnsTrue` [Unit]

### Integration tests — handlers through the policy pipeline

- `AuthorizeAsync_AdministratorAgainstCanModerateComments_DeniesAccess` [Integration]
- `AuthorizeAsync_ModeratorAgainstCanModerateComments_AllowsAccess` [Integration]
