# SDD-FORUM-003 — Registration, Login & Account Activation

## 1. Header

| Field | Value |
|---|---|
| **Title** | Registration, Login & Account Activation |
| **Spec ID** | SDD-FORUM-003 |
| **Category** | core |
| **Status** | Active |
| **Last updated** | 2026-06-21 |
| **Owner** | TBD |

---

## 2. Context & Scope

ForumGuard authenticates and authorizes its participants through **ASP.NET Core Identity**
(`ApplicationUser : IdentityUser<Guid>`). This spec defines the **account lifecycle** that gates
everyone's access to the forum: self-registration, sign-in, and the Administrator-controlled
**activation/deactivation** of accounts.

Three facts shape this spec:

- A new user MUST be able to **self-register** and receives the default Identity role `"User"`.
- An account participates in the forum **only while `ApplicationUser.IsActive == true`**. The
  `IsActive` flag is **administrator-controlled** (never set by the user) and acts as a hard gate
  at **sign-in** and at **comment submission**.
- Only an **Administrator** (authorized by the `CanManageAccounts` policy) MAY toggle another
  account's `IsActive` state.

### Covered

- User self-registration via ASP.NET Core Identity, with `"User"` as the default role.
- The `DisplayName` and `CreatedAtUtc` fields of `ApplicationUser` set at registration.
- Sign-in, including the `IsActive == true` precondition.
- The `IsActive` gate applied to comment submission (the gate only — the submission flow itself
  is owned by `SDD-FORUM-001`).
- Administrator activation/deactivation of accounts via the `CanManageAccounts` policy.
- Effect of deactivation on an already-signed-in session (mid-session deactivation).
- The self-deactivation guard (an Administrator MUST NOT deactivate their own account).

### Excluded

- The definition of the `CanManageAccounts`, `CanManageModerators`, and `CanModerateComments`
  policies, custom requirements/handlers, and `IRoleWorkspace` strategies — owned by
  `SDD-FORUM-011`.
- Granting/revoking the **Moderator** role (the use case) — owned by `SDD-FORUM-004`.
- The comment-submission use case and auto-publish flow — owned by `SDD-FORUM-001`
  (this spec only contributes the `IsActive` precondition to it).
- The moderation pipeline (`SDD-FORUM-021`) and moderator review queue (`SDD-FORUM-002`).
- The canonical `Comment` entity/enum definitions and comment lifecycle state machine — owned by
  `SDD-FORUM-010`.
- Persistence mechanics (repositories, `UnitOfWork`, EF Core mapping for `ApplicationUser`) —
  owned by `SDD-FORUM-022`.
- Password-reset, email-confirmation, external/social login, and two-factor authentication —
  **out of scope** for this practice project.

### Related specs

- `SDD-FORUM-001` — Comment Submission & Auto-Publish Flow (consumes the `IsActive` gate).
- `SDD-FORUM-002` — Moderation Queue & Decisions (downstream of submission/flagging).
- `SDD-FORUM-004` — Moderator Role Management (the Administrator's other duty).
- `SDD-FORUM-010` — Comment Entity & Lifecycle (canonical entities; `ApplicationUser` extension fields).
- `SDD-FORUM-011` — Roles & Authorization Model — defines `CanManageAccounts`.
- `SDD-FORUM-022` — Data Access (Repository + Unit of Work + Specification).

### Key Files (planned)

> **PLANNED / TARGET paths — none of these exist yet.** This is the spec (Phase 1); no source code
> is created in this phase. Paths follow the planned solution structure in `CLAUDE.md` §2.

| Layer | Planned path | Responsibility |
|---|---|---|
| Domain | `src/ForumGuard.Domain/Entities/ApplicationUser.cs` | `ApplicationUser : IdentityUser<Guid>` with `DisplayName`, `IsActive`, `CreatedAtUtc`. |
| Domain | `src/ForumGuard.Domain/Constants/Roles.cs` | Role string constants `"User"`, `"Moderator"`, `"Administrator"`. |
| Application | `src/ForumGuard.Application/Accounts/IAccountService.cs` | Use-case interface: register, set-active state, query active state. |
| Application | `src/ForumGuard.Application/Accounts/AccountService.cs` | Registration, activation/deactivation, self-deactivation guard, `Result<T>` outcomes. |
| Application | `src/ForumGuard.Application/Common/Result.cs` | `Result` / `Result<T>` with Validation/Conflict/NotFound/Forbidden/Unauthorized kinds. |
| Web | `src/ForumGuard.Web/Areas/Identity/Pages/Account/Register.cshtml(.cs)` | Registration Razor Page bound to `UserManager`. |
| Web | `src/ForumGuard.Web/Areas/Identity/Pages/Account/Login.cshtml(.cs)` | Sign-in Razor Page enforcing the `IsActive` precondition. |
| Web | `src/ForumGuard.Web/Pages/Admin/Accounts/Index.cshtml(.cs)` | Admin account list + activate/deactivate actions (guarded by `CanManageAccounts`). |
| Web | `src/ForumGuard.Web/Security/ActiveUserMiddleware.cs` (or `IClaimsTransformation` / cookie validation event) | Re-evaluates `IsActive` per protected request for mid-session deactivation (**B14**). |
| Web | `src/ForumGuard.Web/Program.cs` | Identity registration; default-role seeding; cookie validation hook. |
| Infrastructure | `src/ForumGuard.Infrastructure/Persistence/ForumGuardDbContext.cs` | EF Core mapping for `ApplicationUser` (see `SDD-FORUM-022`). |
| Tests | `tests/ForumGuard.Tests/Accounts/AccountServiceTests.cs` | `[Unit]` tests for registration/activation rules. |
| Tests | `tests/ForumGuard.Tests/Web/AccountFlowsTests.cs` | `[Integration]` tests crossing the Razor request boundary. |

---

## 3. Behavior

RFC-2119 keywords (**MUST**, **SHOULD**, **MAY**) are used. Every MUST/SHOULD below is
independently testable; the Test Plan (§7) maps each to a named test.

### 3.1 Happy path — self-registration

- **B1.** Registration MUST be performed through ASP.NET Core Identity's `UserManager<ApplicationUser>`
  (no direct `DbContext` user inserts in the Web layer).
- **B2.** On successful registration the new `ApplicationUser` MUST be persisted with
  `DisplayName` set from the registration input, `CreatedAtUtc` set to `SYSUTCDATETIME()`-equivalent
  UTC instant at creation, and `IsActive == true`.
- **B3.** Every newly registered account MUST be assigned **exactly** the Identity role `"User"`
  and MUST NOT be assigned `"Moderator"` or `"Administrator"` at registration.
- **B4.** The Web layer MUST NOT accept or bind any caller-supplied value for `IsActive`,
  role membership, `CreatedAtUtc`, or `Id` during registration; these are server-controlled.
- **B5.** A successful registration SHOULD redirect the user to the sign-in experience (or sign
  them in directly) and SHOULD NOT expose the generated `Id` or password hash to the client.

### 3.2 Happy path — sign-in

- **B6.** Sign-in MUST succeed only when **all** of the following hold: the username/email exists,
  the supplied password matches the stored Identity hash, and `ApplicationUser.IsActive == true`.
- **B7.** When sign-in succeeds, the issued authentication principal MUST carry the user's role
  claims (at least `"User"`) so that downstream authorization (`SDD-FORUM-011`) can evaluate them.
- **B8.** Sign-in attempts and their outcomes (success/failure reason class) SHOULD be recorded by
  the application's logging mechanism without logging the plaintext password.

### 3.3 Happy path — administrator activation/deactivation

- **B9.** Toggling another account's `IsActive` flag MUST be authorized by the `CanManageAccounts`
  policy (defined in `SDD-FORUM-011`); only an `"Administrator"` satisfies it.
- **B10.** Deactivating an account MUST set `IsActive = false` and persist the change; reactivating
  MUST set `IsActive = true`. The operation MUST be idempotent with respect to the target value
  (deactivating an already-inactive account leaves it inactive and is not an error).
- **B11.** Deactivation MUST NOT delete, hide, or alter the user's existing `Comment` records;
  previously `Published` and `ApprovedByModerator` comments MUST remain publicly visible per the
  visibility invariant in `SDD-FORUM-010`.

### 3.4 The IsActive gate on comment submission

- **B12.** A comment-submission request from an account whose `IsActive == false` MUST be rejected
  before any analysis or persistence occurs; **no** `Comment` row in `PendingAnalysis` (or any other
  status) is created for an inactive author. (The submission use case is owned by `SDD-FORUM-001`;
  this rule is the gate it MUST enforce.)
- **B13.** A comment-submission request from an account whose `IsActive == true` MUST pass the
  `IsActive` gate and proceed to the `SDD-FORUM-001` flow.

### 3.5 Edge case 1 — mid-session deactivation

- **B14.** When an Administrator deactivates an account that currently holds an active session, the
  **next** authenticated request from that session that requires an authenticated, active user
  (e.g., submitting a comment, opening the user workspace) MUST be blocked as if the user were not
  permitted, rather than served. The application MUST re-evaluate `IsActive` from the persisted store
  on protected requests (i.e., a stale cookie MUST NOT keep granting access to a deactivated user).
- **B15.** A deactivated user SHOULD be signed out / redirected to sign-in on the first blocked
  request, and a subsequent sign-in attempt MUST fail per **B6** until reactivated.

### 3.6 Edge case 2 — administrator self-deactivation guard

- **B16.** An Administrator MUST NOT deactivate **their own** account; such a request MUST be
  rejected as an authorization/conflict error and the target account's `IsActive` MUST remain
  unchanged.
- **B17.** An Administrator MAY deactivate **other** Administrator accounts (the guard is scoped to
  *self* only), subject to **B9**.

### 3.7 Edge case 3 — duplicate registration

- **B18.** A registration request whose email or username already belongs to an existing account
  MUST be rejected as a **conflict**; no second `ApplicationUser` is created and the existing
  account is left unchanged.
- **B19.** A registration whose password does not satisfy the configured Identity password policy
  MUST be rejected as a **validation** error and MUST NOT create an account.

---

## 4. Validation Rules

### 4.1 Field-level

| Field | Rule |
|---|---|
| `Email` | Required; MUST be a syntactically valid email; MUST be unique across `ApplicationUser` (case-insensitive per Identity's normalized email). |
| `UserName` | Required; MUST be unique across `ApplicationUser` (case-insensitive per Identity's normalized name). MAY default to the email when a separate username is not collected. |
| `Password` | Required; MUST satisfy the ASP.NET Core Identity password policy in force (length and complexity per Identity options). Never persisted in plaintext — only the Identity hash. |
| `DisplayName` | Required; non-empty after trim; MUST NOT exceed **128** characters. |
| `IsActive` | Server-controlled. MUST default to `true` at registration. MUST NOT be bound from registration input. |
| `CreatedAtUtc` | Server-controlled UTC instant set at creation; MUST NOT be bound from input. |

### 4.2 Cross-field

- **V1.** Where a password-confirmation field is collected, it MUST equal the `Password` field;
  otherwise the request is a validation error.
- **V2.** Registration input MUST NOT contain `Id`, `IsActive`, `CreatedAtUtc`, or any role field;
  if present, such values MUST be ignored (never trusted).

### 4.3 State-based

- **V3.** Sign-in MUST be permitted only when the resolved `ApplicationUser.IsActive == true`
  (see **B6**); an inactive account fails sign-in regardless of correct credentials.
- **V4.** The activation/deactivation operation MUST only be applied to an existing
  `ApplicationUser`; targeting a non-existent user id is a not-found error.
- **V5.** The self-deactivation guard (**B16**) is a state-based rule: the target user id MUST NOT
  equal the acting Administrator's user id when the requested action is deactivation.
- **V6.** The `IsActive` gate on submission (**B12**) is a state-based precondition evaluated against
  the **persisted** `IsActive` value at request time, not a cached/session value.

---

## 5. Error Rules

The Web layer is **ASP.NET Core Razor Pages** (no JSON/ProblemDetails API contract). The Application
layer surfaces failures via `Result<T>` (or Identity's `IdentityResult` for Identity operations);
Razor handlers translate those into page state and HTTP outcomes. "Domain mapping" below names the
`Result<T>` error kind or exception; "Razor user-facing outcome" names what the user sees.

| # | Trigger | Type | Domain mapping | Razor user-facing outcome |
|---|---|---|---|---|
| **E1** | Registration with an email/username that already exists (**B18**). | conflict | `IdentityResult.Failed` (`DuplicateUserName` / `DuplicateEmail`) surfaced as `Result.Conflict`. | Registration page re-displays with a model error ("An account with this email already exists."); HTTP 200 (page redisplay). No account created. |
| **E2** | Registration password violates the Identity policy, or required field missing/`DisplayName` too long (**B19**, §4.1). | validation | `Result.Validation` carrying field errors (Identity `PasswordTooShort`, etc.). | Page redisplay with per-field validation messages; HTTP 200. No account created. |
| **E3** | Sign-in with wrong password or unknown user (**B6**). | validation (credentials) | `Result.Unauthorized` / Identity `SignInResult.Failed`. | Generic "Invalid login attempt." message (does not reveal which field was wrong); HTTP 200 page redisplay. |
| **E4** | Sign-in attempt on an account where `IsActive == false` (**B6**, **V3**). | authorization (forbidden) | `Result.Forbidden` (account inactive) — distinct from bad credentials. | "This account is deactivated. Contact an administrator." message; sign-in denied; HTTP 200 page redisplay (or 403 on a protected redirect). |
| **E5** | Comment-submission request from an inactive account (**B12**). | authorization (forbidden) | `Result.Forbidden`; no `Comment` persisted. | Submission blocked; user redirected to sign-in or shown a "deactivated account" notice; HTTP 403 / redirect. |
| **E6** | A non-Administrator (or unauthenticated caller) attempts to toggle `IsActive` (**B9**). | authorization | `CanManageAccounts` policy fails → ASP.NET Core authorization rejection. | HTTP 403 Forbidden (or redirect to access-denied page); no change applied. |
| **E7** | Activation/deactivation targets a non-existent user id (**V4**). | not found | `Result.NotFound`. | Access-denied/not-found page or model error on the admin page; HTTP 404 (or page redisplay with error); no change applied. |
| **E8** | An Administrator attempts to deactivate their own account (**B16**, **V5**). | authorization / conflict | `Result.Forbidden` (self-deactivation guard); `IsActive` unchanged. | Admin page redisplay with "You cannot deactivate your own account."; HTTP 200; target unchanged. |
| **E9** | A mid-session request from a now-deactivated user reaches a protected page (**B14**). | authorization | `IsActive` re-check fails → authorization rejection / forced sign-out. | User signed out and redirected to sign-in with an "account deactivated" notice; subsequent sign-in fails via **E4**. |

---

## 6. Versioning Notes

- **v1 — Initial specification (2026-06-21).** Defines self-registration (default `"User"` role,
  server-controlled `IsActive`/`CreatedAtUtc`/`DisplayName`), sign-in with the `IsActive == true`
  precondition, the `IsActive` gate on comment submission, Administrator activation/deactivation via
  the `CanManageAccounts` policy, mid-session deactivation re-evaluation, the administrator
  self-deactivation guard, and duplicate/credential/inactive error rules. Non-breaking (greenfield;
  no prior version).

---

## 7. Test Plan

Tests follow the `MethodName_Scenario_ExpectedResult` convention. Business tests carry
`[Category("SDD-FORUM-003")]`. Each test is tagged `[Unit]` or `[Integration]`. Integration tests
exercise the Razor request boundary (registration, sign-in, admin toggle, mid-session) via the
ASP.NET Core test host.

### 7.1 Registration

- `RegisterAsync_ValidInput_CreatesActiveUserInUserRole` **[Unit]** — verifies **B2**, **B3**: `IsActive == true`, single role `"User"`, `CreatedAtUtc` set.
- `RegisterAsync_AttemptsToSetIsActiveOrRole_IgnoresServerControlledFields` **[Unit]** — verifies **B4**, **V2**: caller-supplied `IsActive`/role/`Id` ignored.
- `RegisterAsync_DuplicateEmail_ReturnsConflict` **[Unit]** — verifies **B18**, **E1**: no second account created.
- `RegisterAsync_WeakPassword_ReturnsValidationError` **[Unit]** — verifies **B19**, **E2**.
- `RegisterAsync_DisplayNameExceeds128Chars_ReturnsValidationError` **[Unit]** — verifies §4.1 `DisplayName` rule.
- `Register_ValidForm_RedirectsAndPersistsUserInUserRole` **[Integration]** — verifies **B1**, **B5** across the Razor boundary.
- `Register_DuplicateEmail_RedisplaysPageWithConflictMessage` **[Integration]** — verifies **E1** user-facing outcome.

### 7.2 Sign-in & the IsActive precondition

- `SignInAsync_ValidCredentialsActiveUser_Succeeds` **[Unit]** — verifies **B6** happy path.
- `SignInAsync_WrongPassword_ReturnsInvalidCredentials` **[Unit]** — verifies **B6**, **E3**.
- `SignInAsync_InactiveAccountCorrectPassword_ReturnsForbidden` **[Unit]** — verifies **B6**, **V3**, **E4** (distinct from bad credentials).
- `Login_ActiveUser_AuthenticatesWithUserRoleClaim` **[Integration]** — verifies **B6**, **B7** across the Razor boundary.
- `Login_DeactivatedAccount_DeniesWithDeactivatedMessage` **[Integration]** — verifies **E4** user-facing outcome.
- `Login_WrongPassword_ShowsGenericInvalidLoginMessage` **[Integration]** — verifies **E3** (no field disclosure).

### 7.3 The IsActive gate on comment submission

- `SubmitComment_InactiveAuthor_RejectedBeforePersistence` **[Unit]** — verifies **B12**, **E5**: no `Comment` row created (gate only; flow in `SDD-FORUM-001`).
- `SubmitComment_ActiveAuthor_PassesIsActiveGate` **[Unit]** — verifies **B13**.
- `SubmitComment_InactiveAuthor_Returns403OrRedirect` **[Integration]** — verifies **E5** across the Razor boundary; asserts no comment persisted.

### 7.4 Administrator activation/deactivation

- `SetActiveAsync_AdministratorDeactivatesOtherUser_SetsIsActiveFalse` **[Unit]** — verifies **B9**, **B10**.
- `SetActiveAsync_DeactivateAlreadyInactiveUser_IsIdempotent` **[Unit]** — verifies **B10** idempotency.
- `SetActiveAsync_DeactivationDoesNotAlterExistingComments_PublishedRemainVisible` **[Unit]** — verifies **B11**.
- `SetActiveAsync_TargetUserNotFound_ReturnsNotFound` **[Unit]** — verifies **V4**, **E7**.
- `SetActiveAsync_AdministratorDeactivatesSelf_ReturnsForbiddenAndLeavesUnchanged` **[Unit]** — verifies **B16**, **V5**, **E8**.
- `SetActiveAsync_AdministratorDeactivatesAnotherAdministrator_Succeeds` **[Unit]** — verifies **B17**.
- `ToggleActive_AsAdministrator_AppliesChange` **[Integration]** — verifies **B9** across the Razor boundary with the `CanManageAccounts` policy satisfied.
- `ToggleActive_AsNonAdministrator_Returns403` **[Integration]** — verifies **B9**, **E6**.
- `ToggleActive_AdministratorTargetsSelf_RedisplaysWithGuardMessage` **[Integration]** — verifies **E8** user-facing outcome.

### 7.5 Mid-session deactivation

- `ProtectedRequest_AfterMidSessionDeactivation_IsBlockedAndSignsOut` **[Integration]** — verifies **B14**, **B15**, **E9**: stale cookie no longer grants access; user redirected to sign-in.
- `SignIn_AfterDeactivation_FailsUntilReactivated` **[Integration]** — verifies **B15** + **E4** after deactivation, and success after reactivation.
