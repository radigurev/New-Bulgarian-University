---
id: SDD-MN-INF-002
title: Authentication & Authorization (ASP.NET Core Identity)
status: Planned
version: 1.0
owner: identity
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-001
  - SDD-MN-DOM-001
  - SDD-MN-INF-003
---

# SDD-MN-INF-002 — Authentication & Authorization (ASP.NET Core Identity)

## 1. Context

MeepleNight uses **ASP.NET Core Identity** with a cookie-based scheme. The
domain `User` entity inherits from `IdentityUser<Guid>` so Identity persists
to the same database (SDD-MN-INF-003).

Two roles exist:

- `User` — assigned automatically on registration.
- `Admin` — seeded for the initial administrator account.

**Key Files (planned):**
- `src/MeepleNight.Web/Program.cs`
- `src/MeepleNight.Data/MeepleDbContext.cs`
- `src/MeepleNight.Data/Migrations/v1.0.0_initial_schema_with_identity.sql`
- `src/MeepleNight.Domain/Entities/User.cs`

## 2. Behavior

### 2.1 Registration (Identity)

- **MUST** register Identity via:

```
builder.Services
  .AddIdentity<User, IdentityRole<Guid>>(options =>
  {
      options.Password.RequireDigit = true;
      options.Password.RequiredLength = 8;
      options.Password.RequireUppercase = true;
      options.Password.RequireNonAlphanumeric = true;
      options.Password.RequireLowercase = false;

      options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
      options.Lockout.MaxFailedAccessAttempts = 5;
      options.Lockout.AllowedForNewUsers = true;

      options.User.RequireUniqueEmail = true;
      options.SignIn.RequireConfirmedAccount = false;
  })
  .AddEntityFrameworkStores<MeepleDbContext>()
  .AddDefaultTokenProviders();
```

- **MUST** configure the Identity cookie:

```
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Cookie.Name = "MeepleNight.Auth";
    options.Cookie.HttpOnly = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.ExpireTimeSpan = TimeSpan.FromDays(14);
    options.SlidingExpiration = true;
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});
```

### 2.2 Authorization Policies

- **MUST** register the following policies via `AddAuthorization`:
  - `RequireAdmin` — `RequireRole("Admin")`. Used by `[Authorize(Policy = "RequireAdmin")]` on every Admin-area controller.
  - `RequireUser` — `RequireAuthenticatedUser()`. Implicit on `[Authorize]` controllers.

- **MUST NOT** rely on `[Authorize(Roles = "...")]` string literals scattered
  through controllers — use named policies for grep-friendliness.

### 2.3 Role Seeding

- On application startup (after migrations apply), **MUST** ensure both `User`
  and `Admin` roles exist in `AspNetRoles`.
- **MUST** seed an initial administrator only if no `Admin` user exists. The
  initial admin's email and password are read from `appsettings.json`
  → `Bootstrap:AdminEmail` / `Bootstrap:AdminPassword`. The bootstrap password
  **MUST** be rotated in production via the password change flow.

### 2.4 CSRF / Antiforgery

- **MUST** apply `[ValidateAntiForgeryToken]` to every POST action that mutates
  state. Razor `Html.BeginForm` includes the token automatically.

### 2.5 Authenticated User Accessor

- **MUST** provide an `ICurrentUserAccessor` service backed by `IHttpContextAccessor`
  that exposes:
  - `Guid? UserId`
  - `string? DisplayName`
  - `bool IsAdmin`
  - `bool IsAuthenticated`
- **MUST** be the only way services read the current user — controllers do not
  pass the user id explicitly to services.

## 3. Validation

| Concern | Rule |
|---|---|
| Password complexity | See SDD-MN-CORE-001 §3 (mirror of Identity options) |
| Email format | Identity's built-in `EmailAddressAttribute` |

## 4. Errors

| Condition | Outcome |
|---|---|
| Anonymous request to `[Authorize]` action | Redirect to `/Account/Login?returnUrl=...` |
| Authenticated non-admin to admin action | Redirect to `/Account/AccessDenied` (HTTP 403 view) |
| Invalid antiforgery token | HTTP 400 with default ASP.NET Core error |

## 5. Versioning

- Tightening password rules requires:
  - A change entry under `docs/changes/`.
  - Communication to existing users — we cannot retroactively re-validate stored
    hashes, so the new rule applies only on next password change.
- Adding a new role (e.g. `Moderator`) requires:
  - A new role-seed step at startup
  - A new policy in `AddAuthorization`
  - Updated tests

## 6. Test Plan

### Unit Tests
- `[Unit] CurrentUserAccessor_AnonymousRequest_UserIdIsNull`
- `[Unit] CurrentUserAccessor_AuthenticatedRequest_ExposesUserIdAndDisplayName`
- `[Unit] PasswordPolicy_ShortPassword_Rejected`
- `[Unit] PasswordPolicy_NoDigit_Rejected`
- `[Unit] PasswordPolicy_NoSymbol_Rejected`

### Integration Tests
- `[Integration] OnStartup_BothRolesExistInDatabase`
- `[Integration] OnStartup_BootstrapAdminCreatedWhenNoneExists`
- `[Integration] OnStartup_BootstrapAdminNotRecreated_WhenOneExists`
- `[Integration] LoginLockout_AfterFiveFailures_BlocksFurtherAttemptsForFiveMinutes`
- `[Integration] AdminController_AnonymousRequest_RedirectsToLogin`
- `[Integration] AdminController_AuthenticatedNonAdmin_ReturnsAccessDenied`

## 7. Cross-References

- Account flows: SDD-MN-CORE-001
- Domain extension: SDD-MN-DOM-001
- Database persistence: SDD-MN-INF-003
