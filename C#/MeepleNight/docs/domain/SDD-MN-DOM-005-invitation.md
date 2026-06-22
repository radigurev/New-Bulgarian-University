---
id: SDD-MN-DOM-005
title: Invitation
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-005
  - SDD-MN-DOM-001
  - SDD-MN-DOM-003
---

# SDD-MN-DOM-005 — Invitation

## 1. Context

An `Invitation` links a `User` (invitee) to a `GameNight`, capturing whether
the invitee has been asked, has accepted, has declined, has been removed, or
had their invitation revoked.

The host of a night also has an Invitation row, auto-created with status
`Accepted` so they appear consistently in attendee lists.

**Key Files (planned):**
- `src/MeepleNight.Domain/Entities/Invitation.cs`
- `src/MeepleNight.Domain/Enums/InvitationStatus.cs`
- `src/MeepleNight.Data/Configurations/InvitationConfiguration.cs`

## 2. Properties

| Property | Type | Required | Notes |
|---|---|---|---|
| Id | Guid | Yes | PK, `NEWSEQUENTIALID()` |
| GameNightId | Guid | Yes | FK to GameNight |
| InviteeUserId | Guid | Yes | FK to User (recipient) |
| InvitedByUserId | Guid | Yes | FK to User (sender — typically the host) |
| Status | enum `InvitationStatus` | Yes | `Pending`, `Accepted`, `Declined`, `Revoked`, `Removed` |
| CreatedAtUtc | datetime2(7) | Yes | Default `SYSUTCDATETIME()` |
| RespondedAtUtc | datetime2(7)? | No | Stamped on first transition out of Pending |

## 3. Relationships

| Relationship | Cardinality | Notes |
|---|---|---|
| Invitation → GameNight | * → 1 | `OnDelete(Cascade)` |
| Invitation → User (Invitee) | * → 1 | `OnDelete(Restrict)` |
| Invitation → User (Inviter) | * → 1 | `OnDelete(Restrict)` |

## 4. Invariants

- The pair `(GameNightId, InviteeUserId)` **MUST** be unique — each user has at
  most one invitation per night.
- `InviteeUserId != InvitedByUserId` is **not** required at the DB level, because
  the host's own auto-acceptance is created by the host inviting themselves
  internally. The Invitations controller (per SDD-MN-CORE-005) refuses
  user-driven self-invitations.
- Allowed status transitions:
  - `Pending → Accepted` (invitee response)
  - `Pending → Declined` (invitee response)
  - `Pending → Revoked` (host cancellation)
  - `Accepted → Declined` (invitee changes mind, while night still planned & future)
  - `Accepted → Removed` (host removes user from attendee roster)
  - `Declined → Accepted` (invitee changes mind, while night still planned & future)
- All other transitions **MUST** be rejected by the service layer.

## 5. EF Core Configuration

```
builder.ToTable("Invitations");
builder.HasKey(i => i.Id);
builder.Property(i => i.Id).HasDefaultValueSql("NEWSEQUENTIALID()");
builder.Property(i => i.Status).HasConversion<string>().HasMaxLength(20);
builder.Property(i => i.CreatedAtUtc).HasColumnType("datetime2(7)").HasDefaultValueSql("SYSUTCDATETIME()");
builder.Property(i => i.RespondedAtUtc).HasColumnType("datetime2(7)");

builder.HasIndex(i => new { i.GameNightId, i.InviteeUserId })
       .IsUnique()
       .HasDatabaseName("UX_Invitations_GameNight_Invitee");
builder.HasIndex(i => i.InviteeUserId).HasDatabaseName("IX_Invitations_InviteeUserId");
builder.HasIndex(i => i.GameNightId).HasDatabaseName("IX_Invitations_GameNightId");

builder.HasOne<GameNight>().WithMany().HasForeignKey(i => i.GameNightId).OnDelete(DeleteBehavior.Cascade);
builder.HasOne<User>().WithMany().HasForeignKey(i => i.InviteeUserId).OnDelete(DeleteBehavior.Restrict);
builder.HasOne<User>().WithMany().HasForeignKey(i => i.InvitedByUserId).OnDelete(DeleteBehavior.Restrict);
```

## 6. Test Plan

### Unit Tests
- `[Unit] Invitation_TransitionPendingToAccepted_StampsRespondedAtUtc`
- `[Unit] Invitation_TransitionRevokedToAccepted_Forbidden`
- `[Unit] Invitation_TransitionRemovedToAccepted_Forbidden`

### Integration Tests
- `[Integration] DuplicateInvitation_SameNightAndInvitee_RejectedByUniqueIndex`
- `[Integration] CascadeDelete_GameNight_RemovesItsInvitations`
- `[Integration] Restrict_DeletingInviteeUser_WithInvitations_Fails`

## 7. Cross-References

- Workflow: SDD-MN-CORE-005
- Parents: SDD-MN-DOM-001 (User), SDD-MN-DOM-003 (GameNight)
