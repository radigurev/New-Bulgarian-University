---
id: SDD-MN-DOM-001
title: User
status: Planned
version: 1.0
owner: identity
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-001
  - SDD-MN-INF-002
---

# SDD-MN-DOM-001 — User

## 1. Context

The `User` is the principal actor in MeepleNight. Every host, attendee, invitee,
and session-player is a User. Authentication and authorisation are handled by
ASP.NET Core Identity (SDD-MN-INF-002), so the application's `User` entity
**extends** `IdentityUser<Guid>` to inherit email, password hash, and security
stamp columns.

**Key Files (planned):**
- `src/MeepleNight.Domain/Entities/User.cs`
- `src/MeepleNight.Data/Configurations/UserConfiguration.cs`

## 2. Properties

| Property | Type | Required | Notes |
|---|---|---|---|
| Id | Guid | Yes | Primary key. Default value `NEWSEQUENTIALID()`. Inherited from `IdentityUser<Guid>`. |
| Email | string(256) | Yes | Unique, normalised. Inherited. |
| EmailConfirmed | bool | Yes | Inherited; not exercised by this app — defaulted to true on registration. |
| PasswordHash | string | Yes | Inherited. |
| SecurityStamp | string | Yes | Inherited. Rotated on password change. |
| DisplayName | string(50) | Yes | App-specific. Visible to other users. |
| AvatarPath | string(260) | No | Optional file path under `wwwroot/uploads/avatars/`. |
| CreatedAtUtc | datetime2(7) | Yes | Stamped on registration; default `SYSUTCDATETIME()`. |
| LastLoginAtUtc | datetime2(7) | No | Updated on every successful login. |

## 3. Relationships

| Relationship | Cardinality | Notes |
|---|---|---|
| User → GameNight (Host) | 1 → * | `GameNight.HostUserId` FK |
| User → Invitation (Invitee) | 1 → * | `Invitation.InviteeUserId` FK |
| User → Invitation (Inviter) | 1 → * | `Invitation.InvitedByUserId` FK |
| User → SessionPlayer | 1 → * | `SessionPlayer.UserId` FK |
| User → Game (CreatedBy) | 1 → * | `Game.CreatedByUserId` FK; nullable when seed data |

## 4. Invariants

- **MUST** have a unique normalised email (Identity-enforced).
- **MUST** have a non-empty DisplayName.
- A user **MUST NOT** be hard-deleted while any `SessionPlayer`, `Invitation`, or
  `GameNight` references them. Deletion path: implement soft-delete via
  `IsDeleted` flag — out of scope for v1.0 (no UI exposes deletion).

## 5. EF Core Configuration

- Use Fluent API in `UserConfiguration : IEntityTypeConfiguration<User>`.
- Inherit from `IdentityUser<Guid>` so Identity infrastructure picks it up.
- Configure:
  ```
  builder.Property(u => u.DisplayName).HasMaxLength(50).IsRequired();
  builder.Property(u => u.AvatarPath).HasMaxLength(260);
  builder.Property(u => u.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
  builder.HasIndex(u => u.NormalizedEmail).IsUnique();
  ```
- `Id` PK uses `NEWSEQUENTIALID()` default — set via `HasDefaultValueSql("NEWSEQUENTIALID()")`.

## 6. Roles

Two roles are seeded by the application on first run (see SDD-MN-INF-002):

- `User` — assigned automatically on registration.
- `Admin` — assigned manually via SQL seed for the initial administrator.

## 7. Test Plan

### Unit Tests
- `[Unit] User_DisplayName_Required`
- `[Unit] User_DefaultsCreatedAtUtc_OnSave`

### Integration Tests
- `[Integration] User_DuplicateEmail_RejectedByUniqueIndex`
- `[Integration] User_CascadeDeletePrevented_WhenReferencedBySession`

## 8. Cross-References

- Auth flows: SDD-MN-CORE-001
- Identity infrastructure: SDD-MN-INF-002
