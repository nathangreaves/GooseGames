IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    IF SCHEMA_ID(N'Fuji') IS NULL EXEC(N'CREATE SCHEMA [Fuji];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE TABLE [Fuji].[Players] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [PlayerNumber] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [ConnectionId] nvarchar(max) NULL,
        [PlayedCardId] uniqueidentifier NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE TABLE [Fuji].[Sessions] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [Password] nvarchar(max) NULL,
        [StatusId] int NOT NULL,
        [SessionMasterId] uniqueidentifier NULL,
        [Score] int NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Sessions_Players_SessionMasterId] FOREIGN KEY ([SessionMasterId]) REFERENCES [Fuji].[Players] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE TABLE [Fuji].[DeckCards] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [FaceValue] int NOT NULL,
        [Order] int NOT NULL,
        CONSTRAINT [PK_DeckCards] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DeckCards_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Fuji].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE TABLE [Fuji].[DiscardedCards] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [FaceValue] int NOT NULL,
        CONSTRAINT [PK_DiscardedCards] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_DiscardedCards_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Fuji].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE TABLE [Fuji].[HandCards] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [FaceValue] int NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_HandCards] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_HandCards_Players_PlayerId] FOREIGN KEY ([PlayerId]) REFERENCES [Fuji].[Players] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_HandCards_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Fuji].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_DeckCards_SessionId] ON [Fuji].[DeckCards] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_DiscardedCards_SessionId] ON [Fuji].[DiscardedCards] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_HandCards_PlayerId] ON [Fuji].[HandCards] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_HandCards_SessionId] ON [Fuji].[HandCards] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_Players_PlayedCardId] ON [Fuji].[Players] ([PlayedCardId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_Players_SessionId] ON [Fuji].[Players] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    CREATE INDEX [IX_Sessions_SessionMasterId] ON [Fuji].[Sessions] ([SessionMasterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    ALTER TABLE [Fuji].[Players] ADD CONSTRAINT [FK_Players_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Fuji].[Sessions] ([Id]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    ALTER TABLE [Fuji].[Players] ADD CONSTRAINT [FK_Players_HandCards_PlayedCardId] FOREIGN KEY ([PlayedCardId]) REFERENCES [Fuji].[HandCards] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502140749_FujiInitial')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200502140749_FujiInitial', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502203649_FujiSessionActivePlayer')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Fuji].[Sessions]') AND [c].[name] = N'Score');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Fuji].[Sessions] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Fuji].[Sessions] DROP COLUMN [Score];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502203649_FujiSessionActivePlayer')
BEGIN
    ALTER TABLE [Fuji].[Sessions] ADD [ActivePlayerId] uniqueidentifier NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502203649_FujiSessionActivePlayer')
BEGIN
    CREATE INDEX [IX_Sessions_ActivePlayerId] ON [Fuji].[Sessions] ([ActivePlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502203649_FujiSessionActivePlayer')
BEGIN
    ALTER TABLE [Fuji].[Sessions] ADD CONSTRAINT [FK_Sessions_Players_ActivePlayerId] FOREIGN KEY ([ActivePlayerId]) REFERENCES [Fuji].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200502203649_FujiSessionActivePlayer')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200502203649_FujiSessionActivePlayer', N'3.1.3');
END;

GO

