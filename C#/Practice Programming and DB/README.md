# ForumGuard

Forum with automatic moderation of rude comments (CSCB634 practice project). An ASP.NET Core **Razor Pages** monolith on **.NET 8**, with **EF Core + SQL Server**, **ASP.NET Core Identity**, and an ML-based comment-moderation pipeline (a keyword profanity pre-filter followed by a trained **NAS-BERT** ML.NET classifier) behind a Chain-of-Responsibility.

## Prerequisites

Only these two — nothing else to configure:

1. **.NET 8 SDK**
2. **SQL Server LocalDB** (ships with Visual Studio and with SQL Server Express)

> **First build is large.** Comment analysis runs the trained NAS-BERT model in-process, so the initial restore downloads the ML runtime (`Microsoft.ML.TorchSharp` + the native `libtorch-cpu`, ~GB) from NuGet automatically. No manual step — but expect a big first restore; subsequent builds are fast. An internet connection is required for that first restore.

## Run

- **Visual Studio:** open `src/ForumGuard.sln`, ensure `ForumGuard.Web` is the startup project, press **F5**. The solution contains only the source and test projects; the documentation (SDD specs, the Part-2 doc, and the explainer) lives in the repo-root `docs/` folder, deliberately kept outside the Visual Studio solution.
- **Command line:** `dotnet run --project src/ForumGuard.Web`

On first run (Development) the database is **created, migrated, and seeded automatically** — no manual setup, no scripts.

## Seeded accounts

Development defaults (logged with a warning — change before any real use):

| Role | Email | Password |
|---|---|---|
| Administrator | `admin@forumguard.local` | `Admin#12345` |
| Moderator | `moderator@forumguard.local` | `Mod#12345` |
| User | `user@forumguard.local` | `User#12345` |

The database is also seeded with **sample content** on first run: four threads with a realistic mix of **published** comments and **flagged** ones already waiting in the moderator queue (authored by the accounts above plus a few community members — Mira, Theo, Sam) — so the forum and the queue are populated from the start.

## Try it

- **User** — post a comment in a thread. A clean comment is published immediately; one containing flagged language (e.g. `idiot`, `trash`) is withheld and sent for review.
- **Moderator** — open **Queue** to approve or reject flagged comments.
- **Administrator** — **Admin → Accounts** (activate/deactivate users) and **Admin → Moderators** (grant/revoke the Moderator role). By design, administrators **cannot** moderate comments.

## Comment analysis

Every comment is analyzed by the trained **NAS-BERT** classifier (ML.NET + TorchSharp), as the assignment requires. Moderation is a Chain of Responsibility: a fast keyword **profanity pre-filter** runs first, then the **NAS-BERT model** scores the comment for toxicity — anything at or above `Moderation:ToxicityThreshold` (default `0.5`) is withheld for moderator review, everything else is published immediately. Because the model runs in-process, the **first build downloads the `libtorch-cpu` native backend (~GB)** via NuGet (automatic, one-time). The offline trainer that produced `Models/sentiment-model.zip` lives in `src/ForumGuard.ModelTrainer` — see its README to retrain on `seed-comments.csv`.

## Layout

```
src/ForumGuard.Domain          domain entities, comment lifecycle, interfaces
src/ForumGuard.Application     moderation pipeline (CoR), role workspaces, options, services
src/ForumGuard.Infrastructure  EF Core + SQL Server, repositories, analyzers
src/ForumGuard.Web             Razor Pages, Identity, authorization, composition root
src/ForumGuard.ModelTrainer    offline NAS-BERT trainer (opt-in)
src/ForumGuard.Tests           NUnit unit + integration tests
docs/                          SDD specifications + Part-2 documentation (outside the VS solution)
```

## Tests

```
dotnet test
```
