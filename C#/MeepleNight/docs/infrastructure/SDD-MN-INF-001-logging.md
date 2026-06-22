---
id: SDD-MN-INF-001
title: Logging (Serilog)
status: Planned
version: 1.0
owner: infrastructure
last_updated: 2026-05-08
related:
  - SDD-MN-CORE-001
  - SDD-MN-CORE-002
  - SDD-MN-CORE-006
---

# SDD-MN-INF-001 — Logging (Serilog)

## 1. Context

MeepleNight uses **Serilog** as the logging framework, replacing the default
`Microsoft.Extensions.Logging` console sink. Serilog is configured at startup
in `Program.cs` and made available via standard `ILogger<T>` injection.

Logs go to two sinks: rolling files on disk for durable record, and console
for development convenience.

**Key Files (planned):**
- `src/MeepleNight.Web/Program.cs`
- `src/MeepleNight.Web/appsettings.json` (Serilog section)

## 2. Behavior

### 2.1 Setup

- **MUST** use `Serilog.AspNetCore` package and replace the default logging
  pipeline via `builder.Host.UseSerilog((ctx, cfg) => cfg.ReadFrom.Configuration(ctx.Configuration))`.
- **MUST** read configuration from `appsettings.json` Serilog section:

```
"Serilog": {
  "MinimumLevel": {
    "Default": "Information",
    "Override": {
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
    }
  },
  "WriteTo": [
    { "Name": "Console" },
    { "Name": "File", "Args": {
        "path": "logs/meeplenight-.log",
        "rollingInterval": "Day",
        "retainedFileCountLimit": 14,
        "outputTemplate": "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {SourceContext} {Message:lj}{NewLine}{Exception}"
    }}
  ],
  "Enrich": [ "FromLogContext", "WithMachineName", "WithThreadId" ]
}
```

- **MUST** call `app.UseSerilogRequestLogging()` after `UseRouting()` so each HTTP
  request logs a single summary line.

### 2.2 What Gets Logged

- **MUST** log at level `Information` for: user registration, login, logout,
  password change, game create/edit/delete, game-night create/cancel, session
  create/delete, invitation send/respond.
- **MUST** log at level `Warning` for: failed login attempts, unauthorised
  access attempts (HTTP 403 paths).
- **MUST** log at level `Error` for: unhandled exceptions, EF Core save failures,
  file-system errors during image upload.
- **MUST** include the current `UserId` and `UserDisplayName` as scoped
  properties on every authenticated request (via a custom middleware that pushes
  `LogContext.PushProperty`).
- **SHOULD** use structured message templates (`logger.LogInformation("Created game {GameId} with title {Title}", id, title)`) — never string interpolation.

### 2.3 Sensitive Data

- **MUST NOT** log raw passwords, password hashes, security stamps, or
  authentication cookies under any circumstance.
- **MUST NOT** log full email addresses at `Information` level — log them at
  `Debug` only when needed for diagnosis. (UserId is sufficient at `Information`.)

### 2.4 Log File Location

- Logs go to `<content-root>/logs/`. The folder is created at startup if missing.
- Log files are gitignored — see `.gitignore` setup in SDD-MN-INF-003.

## 3. Validation

This is configuration only — no user input validation applies.

## 4. Errors

| Condition | Outcome |
|---|---|
| `logs/` folder cannot be created (permission denied) | Application startup fails fast with a clear message; not a soft failure. |
| Disk full at runtime | Serilog drops the log silently (built-in behaviour). The app continues running. |

## 5. Versioning

- Adding a sink (e.g. SQL Server) is a non-breaking change.
- Removing or renaming an enricher is breaking — update dependent dashboards
  and add a change entry.

## 6. Test Plan

### Unit Tests
- `[Unit] LogContextEnricher_AddsUserIdAndDisplayName_ForAuthenticatedRequest`
- `[Unit] LogContextEnricher_DoesNothing_ForAnonymousRequest`

### Integration Tests
- `[Integration] OnApplicationStartup_LogsFolderIsCreated`
- `[Integration] FailedLogin_EmitsWarningWithEmailNotPassword`
- `[Integration] CreatingGame_EmitsInformationLogWithGameId`

## 7. Cross-References

- Logged actions live in: SDD-MN-CORE-001, 002, 004, 005, 006
- Auth context source: SDD-MN-INF-002
