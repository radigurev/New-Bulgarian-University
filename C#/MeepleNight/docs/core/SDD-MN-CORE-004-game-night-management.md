---
id: SDD-MN-CORE-004
title: Game Night Creation & Editing
status: Planned
version: 1.0
owner: planning
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-003
  - SDD-MN-DOM-002
  - SDD-MN-CORE-005
  - SDD-MN-CORE-006
---

# SDD-MN-CORE-004 — Game Night Creation & Editing

## 1. Context

A **game night** is the central planning artefact: a scheduled event with a date,
location, host, optional shortlist of candidate games, and an invitation list.
Hosts use this surface to announce a night; invitees are notified through SDD-MN-CORE-005;
the night becomes loggable through SDD-MN-CORE-006 once it has happened.

**Key Files (planned):**
- `src/MeepleNight.Web/Controllers/GameNightsController.cs`
- `src/MeepleNight.Web/Views/GameNights/{Index,Create,Edit,Details,Delete}.cshtml`
- `src/MeepleNight.Services/GameNightService.cs`
- `src/MeepleNight.Services/Validators/GameNightValidator.cs`

## 2. Behavior

### 2.1 Listing — "My Game Nights"

- **MUST** show, for the authenticated user, two grouped lists:
  - **Hosting** — game nights where this user is the host.
  - **Invited** — game nights where this user has an Accepted or Pending invitation.
- **MUST** sort each group by `ScheduledForUtc` ascending, with past nights at the bottom.
- **MUST** distinguish past, today, and upcoming nights with a visual badge.

### 2.2 Create

- **MUST** require authentication.
- **MUST** capture: Title, ScheduledForUtc (date + time), Location (free text),
  Notes (optional, ≤ 1000 chars), candidate games (multi-select from active catalogue,
  optional, up to 10), expected player count.
- **MUST** set `HostUserId = currentUser.Id` and `CreatedAtUtc = now`.
- **MUST** auto-create an `Invitation` for the host with status `Accepted`
  (so they appear in attendee lists).
- **SHOULD** redirect to the night's Details page on success.

### 2.3 Edit

- **MUST** be allowed only to the host.
- **MUST** allow changing every captured field as long as the night is in
  status `Planned` (not `Cancelled`, not `Completed`).
- **SHOULD** notify accepted invitees on the next page load that the night was
  updated (banner on Details).

### 2.4 Cancel

- **MUST** be allowed only to the host while the night is in status `Planned`.
- **MUST** set `Status = Cancelled` (no row deletion) and stamp `CancelledAtUtc`.
- **MUST** auto-decline all `Pending` invitations associated with the night.

### 2.5 Completion

- A night transitions to `Completed` automatically when the host logs at least one
  session against it (see SDD-MN-CORE-006). Manual completion is **MAY** — a
  "Mark Completed" action available to the host on the Details page.

### 2.6 Details Page

- **MUST** show: title, date/time (in user's local time zone), location, host name,
  candidate games, accepted attendees, pending invitees (host-only view), logged
  sessions, notes.
- **MUST** be visible to: host, all invitees (any status), and admins.
- **MUST** return HTTP 403 to other authenticated users.

## 3. Validation

| Field | Rule |
|---|---|
| Title | Required, 2–100 chars |
| ScheduledForUtc | Required, ≥ now (creation only); for edit ≥ now if night is `Planned` |
| Location | Required, 2–200 chars |
| Notes | Optional, ≤ 1000 chars |
| ExpectedPlayerCount | Required, 2–20 |
| CandidateGameIds | Optional, ≤ 10 distinct ids; each must reference an active Game |

## 4. Errors

| Condition | UI Outcome |
|---|---|
| Non-host attempts Edit | HTTP 403 |
| Edit while night is Completed/Cancelled | Edit page disabled with banner: "This night is already completed/cancelled." |
| Candidate game id is inactive or unknown | Field error: "One of the selected games is no longer available." |
| ScheduledForUtc in the past on Create | Field error |

## 5. Versioning

- Adding a new `GameNightStatus` value (e.g. `Postponed`) requires a migration
  and an entry under `docs/changes/`.
- Removing or renaming an existing status is breaking.

## 6. Test Plan

### Unit Tests
- `[Unit] Create_WithValidPayload_PersistsNightAndAutoAcceptsHostInvitation`
- `[Unit] Create_WithPastDate_ReturnsValidationError`
- `[Unit] Create_WithMoreThanTenCandidateGames_ReturnsValidationError`
- `[Unit] Edit_AsNonHost_ReturnsForbid`
- `[Unit] Edit_WhenCompleted_ReturnsBadRequest`
- `[Unit] Cancel_AsHost_TransitionsToCancelledAndAutoDeclinesPending`
- `[Unit] MarkCompleted_AsHost_TransitionsToCompleted`
- `[Unit] Details_AsUninvitedUser_Returns403`
- `[Unit] MyNights_GroupsHostedAndInvitedSeparately_SortedAscByDate`

### Integration Tests
- `[Integration] CreateNight_PersistsRowAndHostInvitationAccepted`
- `[Integration] CancelNight_AutoDeclinesAllPendingInvitations`

## 7. Cross-References

- Domain: SDD-MN-DOM-003 (GameNight), SDD-MN-DOM-002 (Game), SDD-MN-DOM-005 (Invitation)
- Workflow successors: SDD-MN-CORE-005 (Invitation/RSVP), SDD-MN-CORE-006 (Session Logging)
