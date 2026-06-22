---
id: SDD-MN-CORE-005
title: Invitation & RSVP Workflow
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-005
  - SDD-MN-DOM-003
  - SDD-MN-CORE-004
---

# SDD-MN-CORE-005 — Invitation & RSVP Workflow

## 1. Context

Once a host has created a game night (SDD-MN-CORE-004), they extend invitations
to other registered users. Invitees can accept or decline; their decision affects
attendee counts on the night's Details page and feeds session logging
(SDD-MN-CORE-006).

This is intentionally **internal-user-only** — no email invites, no public links.
Both host and invitee must already have an account.

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/InvitationsController.cs`
- `src/MeepleNight.Web/Views/Invitations/{Inbox,_InviteRow}.cshtml`
- `src/MeepleNight.Services/InvitationService.cs`

## 2. Behavior

### 2.1 Sending — Host action

- **MUST** be available on the Game Night Details page when the requester is the host.
- **MUST** accept a list of one or more user identifiers (selected via an
  autocomplete on email or display name).
- **MUST** create one `Invitation` row per invitee with `Status = Pending` and
  stamp `CreatedAtUtc`, `InvitedByUserId = host.Id`.
- **MUST** silently skip duplicates (a user already invited keeps their existing
  invitation row regardless of status).
- **MUST** refuse to invite the host to their own night via this surface
  (the host's auto-accepted self-invitation is created by SDD-MN-CORE-004).

### 2.2 Cancellation — Host action

- **MUST** allow the host to revoke a `Pending` invitation, setting
  `Status = Revoked` and stamping `RespondedAtUtc`.
- **MUST NOT** allow revoking an `Accepted` invitation directly — instead, the
  host can remove the user from the attendee roster (sets status to `Removed`
  and forbids them from appearing in new sessions logged for this night).

### 2.3 Inbox — Invitee view

- **MUST** be reachable at `/Invitations` for any authenticated user.
- **MUST** show all invitations for the current user, grouped by:
  - **Action needed** — `Pending` invitations sorted by night `ScheduledForUtc` asc
  - **Upcoming** — `Accepted` invitations to nights still in the future
  - **History** — past nights and `Declined` / `Revoked` / `Removed` rows

### 2.4 Accept / Decline — Invitee action

- **MUST** be allowed only when the invitation belongs to the current user AND
  the night is in status `Planned` AND `ScheduledForUtc > now`.
- **MUST** record `Status` and `RespondedAtUtc`.
- **MUST** be idempotent — re-clicking the same response is a no-op (no new row,
  no error).
- **MAY** allow flipping a previous decision (Accepted → Declined or vice versa)
  while the night is still planned and in the future.

### 2.5 Notifications

- **OUT OF SCOPE** for v1.0 — invitations are visible only on the Inbox page and
  the Game Night Details page. No email or push notifications.
- **SHOULD** display a count badge on the navigation header for any user with at
  least one `Pending` invitation.

## 3. Validation

| Field | Rule |
|---|---|
| GameNightId | Required, must reference an existing night where current user is host (for sending) or invitee (for responding) |
| InviteeUserIds | Required, ≥ 1 entry, all entries reference existing users, no entry equals the host's id |
| Response | One of: `Accepted`, `Declined` |

## 4. Errors

| Condition | UI Outcome |
|---|---|
| Inviting self | Field error: "You can't invite yourself." |
| Inviting unknown user id | Field error: "One of the selected users no longer exists." |
| Responding after night completed/cancelled | HTTP 400, message: "This night is no longer accepting RSVPs." |
| Non-invitee trying to respond | HTTP 403 |

## 5. Versioning

- Adding a new `InvitationStatus` value is non-breaking.
- Removing an existing status is breaking — requires a data migration and a
  change entry.

## 6. Test Plan

### Unit Tests
- `[Unit] Send_AsHost_CreatesPendingInvitationsForEachInvitee`
- `[Unit] Send_DuplicateInvitee_DoesNotCreateSecondRow`
- `[Unit] Send_InvitingSelf_ReturnsValidationError`
- `[Unit] Cancel_PendingInvitation_TransitionsToRevoked`
- `[Unit] Respond_AcceptPendingInvitation_RecordsAcceptedAndRespondedAt`
- `[Unit] Respond_DeclinePendingInvitation_RecordsDeclined`
- `[Unit] Respond_AfterNightCompleted_ReturnsBadRequest`
- `[Unit] Respond_AsNonInvitee_ReturnsForbidden`
- `[Unit] Respond_FlipAcceptedToDeclined_AllowedWhileFuturePlanned`
- `[Unit] Inbox_GroupsPendingUpcomingHistory_Correctly`

### Integration Tests
- `[Integration] FullCycle_SendAcceptShowOnDetails_FromTwoUserPerspectives`
- `[Integration] CancelNight_CascadesPendingInvitationsToRevoked`

## 7. Cross-References

- Domain: SDD-MN-DOM-005 (Invitation), SDD-MN-DOM-003 (GameNight), SDD-MN-DOM-001 (User)
- Predecessor: SDD-MN-CORE-004 (Game Night Mgmt — creation triggers self-invitation)
- Successor: SDD-MN-CORE-006 (Session Logging — only accepted attendees appear in player picker)
