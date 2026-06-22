# MeepleNight — Cross-Reference Map

> Last updated: 2026-05-08

This map shows how specs depend on or relate to each other. When changing a spec,
review every spec listed under its row and update them or their tests as needed.

## Core ↔ Domain

| Core Spec | Depends on Domain Specs |
|---|---|
| SDD-MN-CORE-001 (Auth) | SDD-MN-DOM-001 (User) |
| SDD-MN-CORE-002 (Admin Catalog) | SDD-MN-DOM-002 (Game) |
| SDD-MN-CORE-003 (Public Browse) | SDD-MN-DOM-002 (Game) |
| SDD-MN-CORE-004 (Game Night Mgmt) | SDD-MN-DOM-003 (GameNight), SDD-MN-DOM-002 (Game), SDD-MN-DOM-001 (User) |
| SDD-MN-CORE-005 (Invitation/RSVP) | SDD-MN-DOM-005 (Invitation), SDD-MN-DOM-003 (GameNight), SDD-MN-DOM-001 (User) |
| SDD-MN-CORE-006 (Session Logging) | SDD-MN-DOM-004 (Session), SDD-MN-DOM-003 (GameNight), SDD-MN-DOM-002 (Game), SDD-MN-DOM-001 (User) |
| SDD-MN-CORE-007 (Statistics) | SDD-MN-DOM-004 (Session), SDD-MN-DOM-002 (Game) |
| SDD-MN-CORE-008 (Play Next) | SDD-MN-DOM-004 (Session), SDD-MN-DOM-002 (Game), SDD-MN-DOM-001 (User) |

## Core ↔ Infrastructure

| Core Spec | Required Infrastructure |
|---|---|
| All Core specs | SDD-MN-INF-001 (Logging), SDD-MN-INF-003 (Database) |
| SDD-MN-CORE-001..008 (except public browse anonymous path) | SDD-MN-INF-002 (Auth) |
| SDD-MN-CORE-002 (Admin Catalog) | SDD-MN-INF-002 — requires `Admin` role policy |

## Workflow Chains

```
Register/Login (CORE-001)
   ↓
Host creates Game Night (CORE-004)
   ↓
Host sends invitations (CORE-005)
   ↓
Invitees RSVP (CORE-005)
   ↓
Night happens — Host logs sessions (CORE-006)
   ↓
Statistics update (CORE-007) → Play Next suggestion (CORE-008)
```

## Domain Relationships

```
User 1───* GameNight (Host)
User *───* Invitation *───1 GameNight
User *───* Session (via SessionPlayer)
GameNight 1───* Session
Game 1───* Session
Game *───* GameNight (candidate shortlist, optional)
```

## Change Impact Hints

| If you change… | Re-validate… |
|---|---|
| SDD-MN-DOM-002 (Game) | SDD-MN-CORE-002, 003, 004, 006, 007, 008 |
| SDD-MN-DOM-003 (GameNight) | SDD-MN-CORE-004, 005, 006 |
| SDD-MN-DOM-004 (Session) | SDD-MN-CORE-006, 007, 008 |
| SDD-MN-INF-002 (Auth) | All authenticated Core flows |
| SDD-MN-INF-003 (Database) | All Core specs (queries, migrations) |
