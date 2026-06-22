---
id: SDD-MN-INF-003
title: Database Access (EF Core + LocalDB)
status: Planned
version: 1.0
owner: infrastructure
last_updated: 2026-05-08
related:
  - SDD-MN-DOM-001
  - SDD-MN-DOM-002
  - SDD-MN-DOM-003
  - SDD-MN-DOM-004
  - SDD-MN-DOM-005
  - SDD-MN-INF-002
---

# SDD-MN-INF-003 — Database Access (EF Core + LocalDB)

## 1. Context

MeepleNight uses **MS SQL Server LocalDB** (development) or **MS SQL Express**
(deployment) as its persistent store, accessed via **EF Core 8** with
code-first migrations. The same database hosts:

- Identity tables (`AspNetUsers`, `AspNetRoles`, `AspNetUserRoles`, etc.)
- Domain tables (`Games`, `GameNights`, `GameNightCandidates`, `Sessions`, `SessionPlayers`, `Invitations`)

The `MeepleDbContext` extends `IdentityDbContext<User, IdentityRole<Guid>, Guid>`
so EF tracks both worlds in one model.

**Key Files (planned):**
- `src/MeepleNight.Data/MeepleDbContext.cs`
- `src/MeepleNight.Data/Configurations/*Configuration.cs`
- `src/MeepleNight.Data/Migrations/v1.0.0_initial_schema.sql`
- `src/MeepleNight.Data/Repositories/*Repository.cs`
- `src/MeepleNight.Data/Interfaces/I*Repository.cs`

## 2. Behavior

### 2.1 Connection String

- **MUST** read the connection string from `appsettings.json` → `ConnectionStrings:Default`.
- Development default:
  `Server=(localdb)\\MSSQLLocalDB;Database=MeepleNight;Trusted_Connection=True;MultipleActiveResultSets=true;Encrypt=False;`
- Deployment uses SQL Express: `Server=.\\SQLEXPRESS;Database=MeepleNight;Trusted_Connection=True;Encrypt=False;`.

### 2.2 DbContext Registration

```
builder.Services.AddDbContext<MeepleDbContext>(options =>
    options.UseSqlServer(connectionString,
        sql => sql.EnableRetryOnFailure(3, TimeSpan.FromSeconds(5), null)));
```

- **MUST** apply pending migrations on startup in development:
  `await db.Database.MigrateAsync();`. In production, migrations are applied
  manually via `dotnet ef database update` to avoid surprise behaviour on deploy.

### 2.3 Migration Conventions

- **MUST** use raw-SQL versioned migration files under `Migrations/`, named
  `v[Major].[Minor].[Patch]_[ShortDescription].sql`.
- **MUST** include a header comment per file:

```
-- Migration: v1.0.0_initial_schema.sql
-- Date: 2026-05-15
-- Specs: SDD-MN-DOM-001..005, SDD-MN-INF-002
-- Description: Initial schema. Identity tables, Games, GameNights, Sessions, Invitations.
```

- **MUST** wrap every `CREATE`/`DROP` statement in `IF NOT EXISTS` / `IF EXISTS` guards.
- **MUST** use `DATETIME2(7)` with `SYSUTCDATETIME()` defaults for timestamps.
- **MUST** name primary keys `PK_[TableName]`, foreign keys `FK_[Child]_[Parent]`, indexes `IX_[Table]_[Columns]`, unique indexes `UX_[Table]_[Columns]`, check constraints `CK_[Table]_[Concept]`.
- **MUST** use `NEWSEQUENTIALID()` as the default for all `Guid` primary keys.
- **MUST** configure tables and columns explicitly via Fluent API — never rely
  on EF Core naming conventions.
- **MUST NOT** modify a migration script after it has been applied to any
  environment — always create a new versioned script.

### 2.4 Repository Pattern

- **MUST** declare repository interfaces in `MeepleNight.Domain/Interfaces/`
  (so the Domain assembly does not depend on Data). Implementations live in
  `MeepleNight.Data/Repositories/`.
- **MUST** keep repositories thin — typed query methods (e.g. `GetUpcomingNightsForUserAsync`)
  rather than a generic `IRepository<T>` over everything.
- **MAY** use `MeepleDbContext` directly from services for read-only LINQ
  projections that are too specific to belong in a repository (e.g. statistics
  rollups). This is allowed because it keeps the codebase small and readable;
  the alternative would be a fragile parade of repository methods.

### 2.5 Transactions

- **MUST** use a single `SaveChangesAsync` call per request action. Multi-table
  operations that need atomicity (e.g. cancel-night + decline-pending) **MUST**
  use `db.Database.BeginTransactionAsync()`.

### 2.6 Connection Pooling & Retry

- **MUST** enable `EnableRetryOnFailure(3, ...)` to absorb transient LocalDB
  connection drops on dev machines.
- **MUST NOT** wrap manual transactions inside an execution-strategy retry —
  use `dbContext.Database.CreateExecutionStrategy().ExecuteAsync(...)` for those flows.

### 2.7 Backups & Distribution

- For coursework submission, the database **MUST** be exported as a `.bak` file
  via SSMS and stored under `database/MeepleNight.bak`. A README under `database/`
  explains how to attach it.

## 3. Schema Inventory (planned tables)

| Table | Purpose | Spec |
|---|---|---|
| `AspNetUsers` | Identity user (extends with DisplayName, AvatarPath, etc.) | SDD-MN-DOM-001, SDD-MN-INF-002 |
| `AspNetRoles` | `User`, `Admin` | SDD-MN-INF-002 |
| `AspNetUserRoles` | User ↔ Role join | SDD-MN-INF-002 |
| `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserTokens`, `AspNetRoleClaims` | Identity infrastructure | SDD-MN-INF-002 |
| `Games` | Catalogue | SDD-MN-DOM-002 |
| `GameNights` | Scheduled events | SDD-MN-DOM-003 |
| `GameNightCandidates` | Night ↔ Game shortlist | SDD-MN-DOM-003 |
| `Invitations` | Night ↔ User attendance | SDD-MN-DOM-005 |
| `Sessions` | Logged plays | SDD-MN-DOM-004 |
| `SessionPlayers` | Per-player results | SDD-MN-DOM-004 |

## 4. Errors

| Condition | Outcome |
|---|---|
| Connection unavailable on startup | Application startup fails fast with a clear error. |
| Migration fails | Application startup fails fast; logs include the offending migration name and SQL error. |

## 5. Versioning

- Migration files are append-only — each new schema change adds a new versioned file.
- The `MeepleDbContext`'s shape **MUST** match the latest applied migration —
  drift is detected via integration tests that hash the model snapshot.

## 6. Test Plan

### Unit Tests
- `[Unit] DbContext_OnModelCreating_AppliesAllConfigurationsFromAssembly`

### Integration Tests
- `[Integration] StartupMigrate_AppliesAllPendingMigrations_OnEmptyDatabase`
- `[Integration] AllTables_PrimaryKeysAreNamedPK_TableName`
- `[Integration] AllForeignKeys_AreNamedFK_Child_Parent`
- `[Integration] AllGuidPKs_HaveNewSequentialIdDefault`
- `[Integration] AllTimestamps_AreDatetime2_7`
- `[Integration] CreateAllRepositories_CompileAndExecuteSimpleRoundTrip`

## 7. Cross-References

- Domain entities: SDD-MN-DOM-001..005
- Identity hosting: SDD-MN-INF-002
- Cross-cutting logging: SDD-MN-INF-001
