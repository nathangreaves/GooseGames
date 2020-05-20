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

