-- Migration: v1.0.0_initial_schema.sql
-- Date: 2026-05-08
-- Description: Reference SQL for the initial MeepleNight schema. The actual schema
--              is created and maintained via EF Core C# migrations (see Migrations/
--              folder after running `dotnet ef migrations add Initial`). This file is
--              kept as documentation and as a hand-auditable record of the production schema.
--
-- Identity tables (AspNetUsers, AspNetRoles, AspNetUserRoles, AspNetUserClaims,
-- AspNetUserLogins, AspNetUserTokens, AspNetRoleClaims) are created by ASP.NET Core
-- Identity's standard schema and are NOT reproduced here.

-- ============================================================
-- Domain tables
-- ============================================================

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Games')
BEGIN
    CREATE TABLE dbo.Games
    (
        Id                       UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        Title                    NVARCHAR(100)    NOT NULL,
        Description              NVARCHAR(2000)   NULL,
        MinPlayers               INT              NOT NULL,
        MaxPlayers               INT              NOT NULL,
        AverageDurationMinutes   INT              NOT NULL,
        Category                 NVARCHAR(20)     NOT NULL,
        CoverImagePath           NVARCHAR(260)    NULL,
        IsActive                 BIT              NOT NULL DEFAULT (1),
        CreatedAtUtc             DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
        CreatedByUserId          UNIQUEIDENTIFIER NULL,
        UpdatedAtUtc             DATETIME2(7)     NULL,
        UpdatedByUserId          UNIQUEIDENTIFIER NULL,
        CONSTRAINT PK_Games PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT CK_Games_PlayerRange    CHECK (MaxPlayers >= MinPlayers),
        CONSTRAINT CK_Games_Duration       CHECK (AverageDurationMinutes BETWEEN 5 AND 720),
        CONSTRAINT CK_Games_PlayersBounds  CHECK (MinPlayers >= 1 AND MaxPlayers <= 20)
    );

    CREATE INDEX IX_Games_Title ON dbo.Games (Title);
    CREATE INDEX IX_Games_IsActive_Category ON dbo.Games (IsActive, Category);
    CREATE UNIQUE INDEX UX_Games_Title_Active ON dbo.Games (Title) WHERE IsActive = 1;
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GameNights')
BEGIN
    CREATE TABLE dbo.GameNights
    (
        Id                   UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        Title                NVARCHAR(100)    NOT NULL,
        ScheduledForUtc      DATETIME2(7)     NOT NULL,
        Location             NVARCHAR(200)    NOT NULL,
        Notes                NVARCHAR(1000)   NULL,
        ExpectedPlayerCount  INT              NOT NULL,
        HostUserId           UNIQUEIDENTIFIER NOT NULL,
        Status               NVARCHAR(20)     NOT NULL,
        CreatedAtUtc         DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc         DATETIME2(7)     NULL,
        CancelledAtUtc       DATETIME2(7)     NULL,
        CONSTRAINT PK_GameNights PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_GameNights_AspNetUsers FOREIGN KEY (HostUserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE NO ACTION,
        CONSTRAINT CK_GameNights_ExpectedPlayers CHECK (ExpectedPlayerCount BETWEEN 2 AND 20)
    );

    CREATE INDEX IX_GameNights_HostUserId      ON dbo.GameNights (HostUserId);
    CREATE INDEX IX_GameNights_ScheduledForUtc ON dbo.GameNights (ScheduledForUtc);
    CREATE INDEX IX_GameNights_Status          ON dbo.GameNights (Status);
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'GameNightCandidates')
BEGIN
    CREATE TABLE dbo.GameNightCandidates
    (
        GameNightId UNIQUEIDENTIFIER NOT NULL,
        GameId      UNIQUEIDENTIFIER NOT NULL,
        Position    INT              NOT NULL,
        CONSTRAINT PK_GameNightCandidates PRIMARY KEY CLUSTERED (GameNightId, GameId),
        CONSTRAINT FK_GameNightCandidates_GameNights FOREIGN KEY (GameNightId) REFERENCES dbo.GameNights(Id) ON DELETE CASCADE,
        CONSTRAINT FK_GameNightCandidates_Games      FOREIGN KEY (GameId)      REFERENCES dbo.Games(Id)      ON DELETE NO ACTION,
        CONSTRAINT CK_GameNightCandidates_Position CHECK (Position BETWEEN 1 AND 10)
    );
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Invitations')
BEGIN
    CREATE TABLE dbo.Invitations
    (
        Id              UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        GameNightId     UNIQUEIDENTIFIER NOT NULL,
        InviteeUserId   UNIQUEIDENTIFIER NOT NULL,
        InvitedByUserId UNIQUEIDENTIFIER NOT NULL,
        Status          NVARCHAR(20)     NOT NULL,
        CreatedAtUtc    DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
        RespondedAtUtc  DATETIME2(7)     NULL,
        CONSTRAINT PK_Invitations PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Invitations_GameNights              FOREIGN KEY (GameNightId)     REFERENCES dbo.GameNights(Id)  ON DELETE CASCADE,
        CONSTRAINT FK_Invitations_AspNetUsers_Invitee     FOREIGN KEY (InviteeUserId)   REFERENCES dbo.AspNetUsers(Id) ON DELETE NO ACTION,
        CONSTRAINT FK_Invitations_AspNetUsers_InvitedBy   FOREIGN KEY (InvitedByUserId) REFERENCES dbo.AspNetUsers(Id) ON DELETE NO ACTION
    );

    CREATE UNIQUE INDEX UX_Invitations_GameNight_Invitee ON dbo.Invitations (GameNightId, InviteeUserId);
    CREATE INDEX        IX_Invitations_InviteeUserId     ON dbo.Invitations (InviteeUserId);
    CREATE INDEX        IX_Invitations_GameNightId       ON dbo.Invitations (GameNightId);
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Sessions')
BEGIN
    CREATE TABLE dbo.Sessions
    (
        Id                 UNIQUEIDENTIFIER NOT NULL DEFAULT NEWSEQUENTIALID(),
        GameNightId        UNIQUEIDENTIFIER NOT NULL,
        GameId             UNIQUEIDENTIFIER NOT NULL,
        StartedAtUtc       DATETIME2(7)     NOT NULL,
        DurationMinutes    INT              NULL,
        WinnerNote         NVARCHAR(200)    NULL,
        IsCooperativeWin   BIT              NULL,
        CreatedAtUtc       DATETIME2(7)     NOT NULL DEFAULT SYSUTCDATETIME(),
        UpdatedAtUtc       DATETIME2(7)     NULL,
        CONSTRAINT PK_Sessions PRIMARY KEY CLUSTERED (Id),
        CONSTRAINT FK_Sessions_GameNights FOREIGN KEY (GameNightId) REFERENCES dbo.GameNights(Id) ON DELETE CASCADE,
        CONSTRAINT FK_Sessions_Games      FOREIGN KEY (GameId)      REFERENCES dbo.Games(Id)      ON DELETE NO ACTION,
        CONSTRAINT CK_Sessions_Duration   CHECK (DurationMinutes IS NULL OR (DurationMinutes BETWEEN 1 AND 720))
    );

    CREATE INDEX IX_Sessions_GameNightId ON dbo.Sessions (GameNightId);
    CREATE INDEX IX_Sessions_GameId      ON dbo.Sessions (GameId);
END;

IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE name = 'SessionPlayers')
BEGIN
    CREATE TABLE dbo.SessionPlayers
    (
        SessionId UNIQUEIDENTIFIER NOT NULL,
        UserId    UNIQUEIDENTIFIER NOT NULL,
        Score     DECIMAL(9,2)     NULL,
        Placement INT              NOT NULL,
        CONSTRAINT PK_SessionPlayers PRIMARY KEY CLUSTERED (SessionId, UserId),
        CONSTRAINT FK_SessionPlayers_Sessions    FOREIGN KEY (SessionId) REFERENCES dbo.Sessions(Id)     ON DELETE CASCADE,
        CONSTRAINT FK_SessionPlayers_AspNetUsers FOREIGN KEY (UserId)    REFERENCES dbo.AspNetUsers(Id)  ON DELETE NO ACTION,
        CONSTRAINT CK_SessionPlayers_Placement CHECK (Placement >= 1)
    );

    CREATE INDEX IX_SessionPlayers_UserId            ON dbo.SessionPlayers (UserId);
    CREATE INDEX IX_SessionPlayers_UserId_Placement  ON dbo.SessionPlayers (UserId, Placement);
END;
