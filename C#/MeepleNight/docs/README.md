# MeepleNight

A small ASP.NET Core MVC web app for organising **board game nights** with a circle of friends.

## What it does

- Keep a **shared catalogue** of board games (managed by admins).
- Let any registered user **host a game night**, shortlist a few games, and **invite friends**.
- After the night, the host **logs who played what and who won**.
- Those logs feed **personal statistics** and a simple **"play next"** suggestion.

## Roles

| Role | Can do |
|---|---|
| Anonymous | Browse the game catalogue, view game details, register, log in |
| User | Host game nights, invite friends, accept/decline invites, log sessions, see personal stats |
| Admin | Everything a user can, plus manage the game catalogue |

## Glossary

| Term | Meaning |
|---|---|
| **Game** | A board game in the catalogue — has min/max players, average duration, category |
| **Game Night** | A scheduled event with a date, location, host, and a shortlist of candidate games |
| **Invitation** | A host's request for a user to attend a night (Pending / Accepted / Declined) |
| **Session** | One play of a single game during a night — records players, scores, and the winner |
| **Host** | The user who created the night; edits it, invites people, and logs sessions |
| **Attendee** | A user who accepted an invitation |

## How it's built

| Layer | Tech |
|---|---|
| Web | ASP.NET Core MVC (Razor views) |
| Data | EF Core 8 + SQL Server LocalDB |
| Auth | ASP.NET Core Identity (cookies) |
| Logging | Serilog |
| Mapping | AutoMapper |
| Validation | FluentValidation |
| UI | Tailwind CSS + Alpine.js |

See `how-it-works.pdf` in this folder for a one-page walkthrough.

## Running it

```bash
cd src/MeepleNight.Web
dotnet run
```

The app applies migrations and seeds development data on first run, then serves on the URL printed in the console.
