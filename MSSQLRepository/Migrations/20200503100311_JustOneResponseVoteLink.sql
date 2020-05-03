IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    IF SCHEMA_ID(N'JustOne') IS NULL EXEC(N'CREATE SCHEMA [JustOne];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[Responses] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [RoundId] uniqueidentifier NOT NULL,
        [Word] nvarchar(max) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Responses] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[PlayerStatuses] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [Status] uniqueidentifier NOT NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_PlayerStatuses] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[ResponseVotes] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [ResponseId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ResponseVotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ResponseVotes_Responses_ResponseId] FOREIGN KEY ([ResponseId]) REFERENCES [JustOne].[Responses] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[Rounds] (
        [Id] uniqueidentifier NOT NULL,
        [ActivePlayerId] uniqueidentifier NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [WordToGuess] nvarchar(max) NULL,
        [Status] int NOT NULL,
        [Outcome] int NOT NULL,
        CONSTRAINT [PK_Rounds] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[Sessions] (
        [Id] uniqueidentifier NOT NULL,
        [Password] nvarchar(max) NULL,
        [StatusId] int NOT NULL,
        [CurrentRoundId] uniqueidentifier NULL,
        [SessionMasterId] uniqueidentifier NULL,
        [Score] int NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_Rounds_CurrentRoundId] FOREIGN KEY ([CurrentRoundId]) REFERENCES [JustOne].[Rounds] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE TABLE [JustOne].[Players] (
        [Id] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [PlayerNumber] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [ConnectionId] nvarchar(max) NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Players_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [JustOne].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_Players_SessionId] ON [JustOne].[Players] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE UNIQUE INDEX [IX_PlayerStatuses_PlayerId] ON [JustOne].[PlayerStatuses] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_ResponseVotes_PlayerId] ON [JustOne].[ResponseVotes] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_ResponseVotes_ResponseId] ON [JustOne].[ResponseVotes] ([ResponseId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_Rounds_ActivePlayerId] ON [JustOne].[Rounds] ([ActivePlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_Rounds_SessionId] ON [JustOne].[Rounds] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_Sessions_CurrentRoundId] ON [JustOne].[Sessions] ([CurrentRoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    CREATE INDEX [IX_Sessions_SessionMasterId] ON [JustOne].[Sessions] ([SessionMasterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    ALTER TABLE [JustOne].[PlayerStatuses] ADD CONSTRAINT [FK_PlayerStatuses_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    ALTER TABLE [JustOne].[ResponseVotes] ADD CONSTRAINT [FK_ResponseVotes_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    ALTER TABLE [JustOne].[Rounds] ADD CONSTRAINT [FK_Rounds_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [JustOne].[Sessions] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    ALTER TABLE [JustOne].[Rounds] ADD CONSTRAINT [FK_Rounds_Players_ActivePlayerId] FOREIGN KEY ([ActivePlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    ALTER TABLE [JustOne].[Sessions] ADD CONSTRAINT [FK_Sessions_Players_SessionMasterId] FOREIGN KEY ([SessionMasterId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200330211301_Initial-Entities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200330211301_Initial-Entities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[Sessions] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[Rounds] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[ResponseVotes] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[Responses] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[PlayerStatuses] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    ALTER TABLE [JustOne].[Players] ADD [CreatedUtc] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409162732_CreatedUtc')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200409162732_CreatedUtc', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409180233_LinkFromResponseToOtherEntities')
BEGIN
    CREATE INDEX [IX_Responses_PlayerId] ON [JustOne].[Responses] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409180233_LinkFromResponseToOtherEntities')
BEGIN
    CREATE INDEX [IX_Responses_RoundId] ON [JustOne].[Responses] ([RoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409180233_LinkFromResponseToOtherEntities')
BEGIN
    ALTER TABLE [JustOne].[Responses] ADD CONSTRAINT [FK_Responses_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [JustOne].[Players] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409180233_LinkFromResponseToOtherEntities')
BEGIN
    ALTER TABLE [JustOne].[Responses] ADD CONSTRAINT [FK_Responses_Rounds_RoundId] FOREIGN KEY ([RoundId]) REFERENCES [JustOne].[Rounds] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200409180233_LinkFromResponseToOtherEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200409180233_LinkFromResponseToOtherEntities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200503100311_JustOneResponseVoteLink')
BEGIN
    ALTER TABLE [JustOne].[ResponseVotes] DROP CONSTRAINT [FK_ResponseVotes_Responses_ResponseId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200503100311_JustOneResponseVoteLink')
BEGIN
    ALTER TABLE [JustOne].[ResponseVotes] ADD CONSTRAINT [FK_ResponseVotes_Responses_ResponseId] FOREIGN KEY ([ResponseId]) REFERENCES [JustOne].[Responses] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200503100311_JustOneResponseVoteLink')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200503100311_JustOneResponseVoteLink', N'3.1.3');
END;

GO

