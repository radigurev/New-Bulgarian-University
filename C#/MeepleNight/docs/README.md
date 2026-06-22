# MeepleNight — Documentation Index

> Last updated: 2026-05-08
> Status: Planned (university coursework project)

## Project Description

**MeepleNight** is an ASP.NET Core MVC web application that helps a group of friends organise
**board game nights**. The system has three pillars:

1. A **shared catalogue** of board games (curated by administrators).
2. A **planning surface** where any registered user can host a game night, pick which games might be played, and invite friends.
3. A **logbook** where the host records who actually played which game and who won — feeding personal statistics and a "play next" recommendation.

The project demonstrates a typical multi-role MVC application with:

- Public anonymous browsing of the game catalogue
- Authenticated user actions (host, invite, RSVP, log)
- Administrator-only catalogue management
- Many-to-many relationships (Sessions ↔ Players, Game Nights ↔ Invitees)

## Tech Stack

See [`CLAUDE.md`](../CLAUDE.md) at the project root.

## How to Read These Docs

Each spec is a markdown file with a `SDD-MN-<area>-<number>` ID. Specs are grouped by category:

| Folder | Contents |
|---|---|
| [`core/`](core/) | Feature and use-case specs (what the app does) |
| [`domain/`](domain/) | Entity, value-object, and business-rule specs |
| [`infrastructure/`](infrastructure/) | Cross-cutting concerns: logging, authentication, database |
| [`integration/`](integration/) | External system integrations (none for this project) |

Cross-spec relationships are tracked in [`cross-reference-map.md`](cross-reference-map.md).
Significant changes are recorded under [`changes/`](changes/) using [`_TEMPLATE.md`](changes/_TEMPLATE.md).

## Spec Catalogue

### Core (Features & Use Cases)

| ID | Title | Status |
|---|---|---|
| [SDD-MN-CORE-001](core/SDD-MN-CORE-001-user-registration-and-authentication.md) | User Registration & Authentication | Planned |
| [SDD-MN-CORE-002](core/SDD-MN-CORE-002-admin-game-catalog.md) | Admin Game Catalog Management | Planned |
| [SDD-MN-CORE-003](core/SDD-MN-CORE-003-public-game-browsing.md) | Public Game Browsing & Search | Planned |
| [SDD-MN-CORE-004](core/SDD-MN-CORE-004-game-night-management.md) | Game Night Creation & Editing | Planned |
| [SDD-MN-CORE-005](core/SDD-MN-CORE-005-invitation-rsvp.md) | Invitation & RSVP Workflow | Planned |
| [SDD-MN-CORE-006](core/SDD-MN-CORE-006-session-logging.md) | Game Session Logging | Planned |
| [SDD-MN-CORE-007](core/SDD-MN-CORE-007-personal-statistics.md) | Personal Statistics Dashboard | Planned |
| [SDD-MN-CORE-008](core/SDD-MN-CORE-008-play-next-suggestion.md) | "Play Next" Suggestion Engine | Planned |

### Domain (Entities)

| ID | Title | Status |
|---|---|---|
| [SDD-MN-DOM-001](domain/SDD-MN-DOM-001-user.md) | User | Planned |
| [SDD-MN-DOM-002](domain/SDD-MN-DOM-002-game.md) | Game | Planned |
| [SDD-MN-DOM-003](domain/SDD-MN-DOM-003-game-night.md) | GameNight | Planned |
| [SDD-MN-DOM-004](domain/SDD-MN-DOM-004-session.md) | Session & SessionPlayer | Planned |
| [SDD-MN-DOM-005](domain/SDD-MN-DOM-005-invitation.md) | Invitation | Planned |

### Infrastructure

| ID | Title | Status |
|---|---|---|
| [SDD-MN-INF-001](infrastructure/SDD-MN-INF-001-logging.md) | Logging (Serilog) | Planned |
| [SDD-MN-INF-002](infrastructure/SDD-MN-INF-002-authentication.md) | Authentication & Authorization (Identity) | Planned |
| [SDD-MN-INF-003](infrastructure/SDD-MN-INF-003-database.md) | Database Access (EF Core + LocalDB) | Planned |

## User Roles

| Role | Capabilities |
|---|---|
| Anonymous | Browse public game catalogue, view game detail pages, register, log in |
| User (registered) | All anonymous capabilities, plus: host game nights, invite friends, accept/decline invitations, log sessions for nights they hosted, view personal stats |
| Admin | All user capabilities, plus: manage the game catalogue (CRUD on games), soft-delete games |

## Glossary

| Term | Definition |
|---|---|
| **Game** | A board game in the shared catalogue (e.g. *Catan*, *Wingspan*). Has min/max players, average duration, category. |
| **Game Night** | A scheduled event with date, location, host, and an optional shortlist of candidate games. |
| **Invitation** | A request from a host to a user to attend a specific game night. Has a status (Pending / Accepted / Declined). |
| **Session** | An actual play of a single game during a game night. Records the players, scores, and the winner. |
| **Host** | The user who created a game night. Can edit it, invite people, and log sessions. |
| **Attendee** | A user who has accepted an invitation. |
