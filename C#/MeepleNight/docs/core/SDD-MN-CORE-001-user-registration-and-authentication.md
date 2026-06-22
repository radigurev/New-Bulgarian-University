---
id: SDD-MN-CORE-001
title: User Registration & Authentication
status: Planned
version: 1.0
owner: identity
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-001
  - SDD-MN-INF-002
---

# SDD-MN-CORE-001 — User Registration & Authentication

## 1. Context

MeepleNight is a private social application — to host game nights, RSVP to invitations,
or log session results, a user must have an authenticated account. Anonymous visitors
are still welcome to browse the public game catalogue (see SDD-MN-CORE-003).

This spec defines the registration, login, logout, and password-management flows
on top of ASP.NET Core Identity (see SDD-MN-INF-002).

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/AccountController.cs`
- `src/MeepleNight.Web/Views/Account/Register.cshtml`
- `src/MeepleNight.Web/Views/Account/Login.cshtml`
- `src/MeepleNight.Web/ViewModels/Account/*`

## 2. Behavior

### 2.1 Registration

- **MUST** allow any anonymous visitor to register a new account by providing
  email, display name, password, and password confirmation.
- **MUST** enforce that email addresses are unique across all users.
- **MUST** assign every newly registered user the `User` role by default.
  Promotion to `Admin` is performed manually in the database (out of scope for the UI).
- **MUST** redirect a successfully registered user to the home page and sign them
  in automatically.
- **SHOULD** present validation errors inline next to the offending field, not as
  a single blob at the top of the form.

### 2.2 Login

- **MUST** authenticate users by email + password.
- **MUST** support "remember me" via a persistent cookie when the box is checked.
- **MUST** lock an account for 5 minutes after 5 consecutive failed login attempts
  within a 5-minute sliding window.
- **MUST** redirect post-login to the original requested URL when one is supplied
  via the `returnUrl` parameter (subject to local-URL validation — external URLs
  fall back to the home page).
- **SHOULD NOT** disclose whether a failed login was caused by a missing email vs
  a wrong password — both produce a generic "invalid credentials" message.

### 2.3 Logout

- **MUST** clear the authentication cookie and redirect to the home page.
- **MUST** be reachable only via HTTP POST (CSRF-protected).

### 2.4 Password Change

- **MUST** require the current password before accepting a new one.
- **MUST** invalidate all existing authentication cookies on successful change so
  other browsers / sessions are signed out.

## 3. Validation

| Field | Rule |
|---|---|
| Email | Required, valid format, ≤ 256 chars, unique |
| DisplayName | Required, 2–50 chars, allowed: letters, digits, spaces, `-`, `_`, `.` |
| Password | Required, ≥ 8 chars, ≥ 1 uppercase, ≥ 1 digit, ≥ 1 non-alphanumeric |
| ConfirmPassword | Must equal Password |
| RememberMe | Optional boolean |

## 4. Errors

| Condition | UI Outcome |
|---|---|
| Email already in use | Form re-rendered with field error: "An account with this email already exists." |
| Password fails complexity | Field error listing failed rules |
| Invalid login | Generic message: "Invalid email or password." |
| Account locked | Message: "Account temporarily locked. Try again in N minutes." |
| Password change with wrong current password | Field error on `CurrentPassword`: "Current password is incorrect." |

## 5. Versioning

Breaking changes (e.g. tightening password rules) require:
- A new migration to rotate Identity stamps if applicable.
- A change entry under `docs/changes/`.
- Communication to existing users at next login.

## 6. Test Plan

### Unit Tests
- `[Unit] Register_WithValidData_CreatesUserAndAssignsUserRole`
- `[Unit] Register_WithDuplicateEmail_ReturnsViewWithError`
- `[Unit] Register_WithWeakPassword_ReturnsViewWithError`
- `[Unit] Login_WithCorrectCredentials_SignsInAndRedirects`
- `[Unit] Login_WithWrongPassword_ReturnsGenericError`
- `[Unit] Login_AfterFiveFailedAttempts_LocksAccount`
- `[Unit] Login_WithExternalReturnUrl_FallsBackToHome`
- `[Unit] ChangePassword_WithWrongCurrent_ReturnsError`
- `[Unit] ChangePassword_OnSuccess_InvalidatesOtherCookies`

### Integration Tests
- `[Integration] Register_PersistsAspNetUserRow_WithUserRole`
- `[Integration] Login_SetsAuthCookie`
- `[Integration] Logout_ClearsAuthCookie`

## 7. Cross-References

- Domain: SDD-MN-DOM-001 (User)
- Infrastructure: SDD-MN-INF-002 (Identity), SDD-MN-INF-001 (Logging — log register/login/logout events)
