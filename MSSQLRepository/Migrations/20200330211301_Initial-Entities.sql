IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF SCHEMA_ID(N'JustOne') IS NULL EXEC(N'CREATE SCHEMA [JustOne];');

GO

CREATE TABLE [JustOne].[Responses] (
    [Id] uniqueidentifier NOT NULL,
    [PlayerId] uniqueidentifier NOT NULL,
    [RoundId] uniqueidentifier NOT NULL,
    [Word] nvarchar(max) NULL,
    [Status] int NOT NULL,
    CONSTRAINT [PK_Responses] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [JustOne].[PlayerStatuses] (
    [Id] uniqueidentifier NOT NULL,
    [PlayerId] uniqueidentifier NOT NULL,
    [Status] uniqueidentifier NOT NULL,
    [Description] nvarchar(max) NULL,
    CONSTRAINT [PK_PlayerStatuses] PRIMARY KEY ([Id])
);

GO

CREATE TABLE [JustOne].[ResponseVotes] (
    [Id] uniqueidentifier NOT NULL,
    [PlayerId] uniqueidentifier NOT NULL,
    [ResponseId] uniqueidentifier NOT NULL,
    CONSTRAINT [PK_ResponseVotes] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_ResponseVotes_Responses_ResponseId] FOREIGN KEY ([ResponseId]) REFERENCES [JustOne].[Responses] ([Id]) ON DELETE CASCADE
);

GO

CREATE TABLE [JustOne].[Rounds] (
    [Id] uniqueidentifier NOT NULL,
    [ActivePlayerId] uniqueidentifier NULL,
    [SessionId] uniqueidentifier NOT NULL,
    [WordToGuess] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [Outcome] int NOT NULL,
    CONSTRAINT [PK_Rounds] PRIMARY KEY ([Id])
);

GO

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

GO

CREATE TABLE [JustOne].[Players] (
    [Id] uniqueidentifier NOT NULL,
    [SessionId] uniqueidentifier NOT NULL,
    [PlayerNumber] int NOT NULL,
    [Name] nvarchar(max) NULL,
    [ConnectionId] nvarchar(max) NULL,
    CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
    CONSTRAINT [FK_Players_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [JustOne].[Sessions] ([Id]) ON DELETE CASCADE
);

GO

CREATE INDEX [IX_Players_SessionId] ON [JustOne].[Players] ([SessionId]);

GO

CREATE UNIQUE INDEX [IX_PlayerStatuses_PlayerId] ON [JustOne].[PlayerStatuses] ([PlayerId]);

GO

CREATE INDEX [IX_ResponseVotes_PlayerId] ON [JustOne].[ResponseVotes] ([PlayerId]);

GO

CREATE INDEX [IX_ResponseVotes_ResponseId] ON [JustOne].[ResponseVotes] ([ResponseId]);

GO

CREATE INDEX [IX_Rounds_ActivePlayerId] ON [JustOne].[Rounds] ([ActivePlayerId]);

GO

CREATE INDEX [IX_Rounds_SessionId] ON [JustOne].[Rounds] ([SessionId]);

GO

CREATE INDEX [IX_Sessions_CurrentRoundId] ON [JustOne].[Sessions] ([CurrentRoundId]);

GO

CREATE INDEX [IX_Sessions_SessionMasterId] ON [JustOne].[Sessions] ([SessionMasterId]);

GO

ALTER TABLE [JustOne].[PlayerStatuses] ADD CONSTRAINT [FK_PlayerStatuses_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [JustOne].[ResponseVotes] ADD CONSTRAINT [FK_ResponseVotes_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [JustOne].[Rounds] ADD CONSTRAINT [FK_Rounds_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [JustOne].[Sessions] ([Id]) ON DELETE CASCADE;

GO

ALTER TABLE [JustOne].[Rounds] ADD CONSTRAINT [FK_Rounds_Players_ActivePlayerId] FOREIGN KEY ([ActivePlayerId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE NO ACTION;

GO

ALTER TABLE [JustOne].[Sessions] ADD CONSTRAINT [FK_Sessions_Players_SessionMasterId] FOREIGN KEY ([SessionMasterId]) REFERENCES [JustOne].[Players] ([Id]) ON DELETE NO ACTION;

GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20200330211301_Initial-Entities', N'3.1.3');

GO

