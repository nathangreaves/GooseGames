IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    IF SCHEMA_ID(N'Avalon') IS NULL EXEC(N'CREATE SCHEMA [Avalon];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE TABLE [Avalon].[Games] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [GodPlayerId] uniqueidentifier NOT NULL,
        [NumberOfPlayers] int NOT NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE TABLE [Avalon].[GameRoles] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [RoleEnum] int NOT NULL,
        CONSTRAINT [PK_GameRoles] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_GameRoles_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Avalon].[Games] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE TABLE [Avalon].[PlayerIntel] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [IntelType] int NOT NULL,
        [IntelPlayerId] uniqueidentifier NULL,
        [IntelNumber] int NULL,
        [RoleKnowsYou] int NULL,
        CONSTRAINT [PK_PlayerIntel] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PlayerIntel_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Avalon].[Games] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE TABLE [Avalon].[PlayerStates] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [GameRoleId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_PlayerStates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PlayerStates_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [Avalon].[Games] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_PlayerStates_GameRoles_GameRoleId] FOREIGN KEY ([GameRoleId]) REFERENCES [Avalon].[GameRoles] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_GameRoles_GameId] ON [Avalon].[GameRoles] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerIntel_GameId] ON [Avalon].[PlayerIntel] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerIntel_PlayerId] ON [Avalon].[PlayerIntel] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_GameId] ON [Avalon].[PlayerStates] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_GameRoleId] ON [Avalon].[PlayerStates] ([GameRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_PlayerId] ON [Avalon].[PlayerStates] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210202233308_InitialEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210202233308_InitialEntities', N'3.1.3');
END;

GO

