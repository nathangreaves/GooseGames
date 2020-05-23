IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    IF SCHEMA_ID(N'Werewords') IS NULL EXEC(N'CREATE SCHEMA [Werewords];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE TABLE [Werewords].[PlayerRoundInformation] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [RoundId] uniqueidentifier NOT NULL,
        [SecretRole] int NOT NULL,
        [Ticks] int NOT NULL,
        [Crosses] int NOT NULL,
        [QuestionMarks] int NOT NULL,
        [SoClose] int NOT NULL,
        [Correct] int NOT NULL,
        CONSTRAINT [PK_PlayerRoundInformation] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE TABLE [Werewords].[PlayerVotes] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [RoundId] uniqueidentifier NOT NULL,
        [VotedPlayerId] uniqueidentifier NOT NULL,
        [VoteType] int NOT NULL,
        CONSTRAINT [PK_PlayerVotes] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE TABLE [Werewords].[Rounds] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [MayorId] uniqueidentifier NULL,
        [SecretWord] nvarchar(max) NULL,
        [RoundStartedUtc] datetime2 NOT NULL,
        [RoundDurationMinutes] int NOT NULL,
        [Status] int NOT NULL,
        [Outcome] int NOT NULL,
        CONSTRAINT [PK_Rounds] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE TABLE [Werewords].[Sessions] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [Password] nvarchar(max) NULL,
        [StatusId] int NOT NULL,
        [SessionMasterId] uniqueidentifier NULL,
        [CurrentRoundId] uniqueidentifier NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_Rounds_CurrentRoundId] FOREIGN KEY ([CurrentRoundId]) REFERENCES [Werewords].[Rounds] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE TABLE [Werewords].[Players] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [PlayerNumber] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [ConnectionId] nvarchar(max) NULL,
        [Status] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Players_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Werewords].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_PlayerRoundInformation_PlayerId] ON [Werewords].[PlayerRoundInformation] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_PlayerRoundInformation_RoundId] ON [Werewords].[PlayerRoundInformation] ([RoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_Players_SessionId] ON [Werewords].[Players] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_PlayerVotes_PlayerId] ON [Werewords].[PlayerVotes] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_PlayerVotes_RoundId] ON [Werewords].[PlayerVotes] ([RoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_PlayerVotes_VotedPlayerId] ON [Werewords].[PlayerVotes] ([VotedPlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_Rounds_MayorId] ON [Werewords].[Rounds] ([MayorId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_Rounds_SessionId] ON [Werewords].[Rounds] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_Sessions_CurrentRoundId] ON [Werewords].[Sessions] ([CurrentRoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    CREATE INDEX [IX_Sessions_SessionMasterId] ON [Werewords].[Sessions] ([SessionMasterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[PlayerRoundInformation] ADD CONSTRAINT [FK_PlayerRoundInformation_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Werewords].[Players] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[PlayerRoundInformation] ADD CONSTRAINT [FK_PlayerRoundInformation_Rounds_RoundId] FOREIGN KEY ([RoundId]) REFERENCES [Werewords].[Rounds] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[PlayerVotes] ADD CONSTRAINT [FK_PlayerVotes_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Werewords].[Players] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[PlayerVotes] ADD CONSTRAINT [FK_PlayerVotes_Players_VotedPlayerId] FOREIGN KEY ([VotedPlayerId]) REFERENCES [Werewords].[Players] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[PlayerVotes] ADD CONSTRAINT [FK_PlayerVotes_Rounds_RoundId] FOREIGN KEY ([RoundId]) REFERENCES [Werewords].[Rounds] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD CONSTRAINT [FK_Rounds_Players_MayorId] FOREIGN KEY ([MayorId]) REFERENCES [Werewords].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD CONSTRAINT [FK_Rounds_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Werewords].[Sessions] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    ALTER TABLE [Werewords].[Sessions] ADD CONSTRAINT [FK_Sessions_Players_SessionMasterId] FOREIGN KEY ([SessionMasterId]) REFERENCES [Werewords].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200516095551_WerewordsEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200516095551_WerewordsEntities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200520211030_IsMayorColumn')
BEGIN
    ALTER TABLE [Werewords].[PlayerRoundInformation] ADD [IsMayor] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200520211030_IsMayorColumn')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200520211030_IsMayorColumn', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Werewords].[PlayerRoundInformation]') AND [c].[name] = N'Correct');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Werewords].[PlayerRoundInformation] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Werewords].[PlayerRoundInformation] DROP COLUMN [Correct];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Werewords].[PlayerRoundInformation]') AND [c].[name] = N'Crosses');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Werewords].[PlayerRoundInformation] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [Werewords].[PlayerRoundInformation] DROP COLUMN [Crosses];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    DECLARE @var2 sysname;
    SELECT @var2 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Werewords].[PlayerRoundInformation]') AND [c].[name] = N'QuestionMarks');
    IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Werewords].[PlayerRoundInformation] DROP CONSTRAINT [' + @var2 + '];');
    ALTER TABLE [Werewords].[PlayerRoundInformation] DROP COLUMN [QuestionMarks];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    DECLARE @var3 sysname;
    SELECT @var3 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Werewords].[PlayerRoundInformation]') AND [c].[name] = N'SoClose');
    IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Werewords].[PlayerRoundInformation] DROP CONSTRAINT [' + @var3 + '];');
    ALTER TABLE [Werewords].[PlayerRoundInformation] DROP COLUMN [SoClose];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    DECLARE @var4 sysname;
    SELECT @var4 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Werewords].[PlayerRoundInformation]') AND [c].[name] = N'Ticks');
    IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [Werewords].[PlayerRoundInformation] DROP CONSTRAINT [' + @var4 + '];');
    ALTER TABLE [Werewords].[PlayerRoundInformation] DROP COLUMN [Ticks];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    CREATE TABLE [Werewords].[PlayerResponses] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [PlayerRoundInformationId] uniqueidentifier NOT NULL,
        [ResponseType] int NOT NULL,
        CONSTRAINT [PK_PlayerResponses] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PlayerResponses_PlayerRoundInformation_PlayerRoundInformationId] FOREIGN KEY ([PlayerRoundInformationId]) REFERENCES [Werewords].[PlayerRoundInformation] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    CREATE INDEX [IX_PlayerResponses_PlayerRoundInformationId] ON [Werewords].[PlayerResponses] ([PlayerRoundInformationId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522093652_AddResponseTable')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200522093652_AddResponseTable', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522134337_AddVoteTime')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD [VoteDurationSeconds] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522134337_AddVoteTime')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD [VoteStartedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522134337_AddVoteTime')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200522134337_AddVoteTime', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522222804_TokensSpent')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD [SoCloseSpent] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522222804_TokensSpent')
BEGIN
    ALTER TABLE [Werewords].[Rounds] ADD [WayOffSpent] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200522222804_TokensSpent')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200522222804_TokensSpent', N'3.1.3');
END;

GO

