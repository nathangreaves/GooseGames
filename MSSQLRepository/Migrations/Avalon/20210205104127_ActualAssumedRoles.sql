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

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    ALTER TABLE [Avalon].[PlayerStates] DROP CONSTRAINT [FK_PlayerStates_GameRoles_GameRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    DROP INDEX [IX_PlayerStates_GameRoleId] ON [Avalon].[PlayerStates];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    ALTER TABLE [Avalon].[PlayerStates] ADD [ActualRoleId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    ALTER TABLE [Avalon].[PlayerStates] ADD [AssumedRoleId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    update Avalon.PlayerStates set ActualRoleId = GameRoleId, AssumedRoleId = GameRoleId where 1 = 1
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Avalon].[PlayerStates]') AND [c].[name] = N'GameRoleId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Avalon].[PlayerStates] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Avalon].[PlayerStates] DROP COLUMN [GameRoleId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    CREATE INDEX [IX_PlayerStates_ActualRoleId] ON [Avalon].[PlayerStates] ([ActualRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    CREATE INDEX [IX_PlayerStates_AssumedRoleId] ON [Avalon].[PlayerStates] ([AssumedRoleId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    ALTER TABLE [Avalon].[PlayerStates] ADD CONSTRAINT [FK_PlayerStates_GameRoles_ActualRoleId] FOREIGN KEY ([ActualRoleId]) REFERENCES [Avalon].[GameRoles] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    ALTER TABLE [Avalon].[PlayerStates] ADD CONSTRAINT [FK_PlayerStates_GameRoles_AssumedRoleId] FOREIGN KEY ([AssumedRoleId]) REFERENCES [Avalon].[GameRoles] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20210205104127_ActualAssumedRoles')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20210205104127_ActualAssumedRoles', N'3.1.3');
END;

GO

