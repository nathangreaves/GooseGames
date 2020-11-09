IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    IF SCHEMA_ID(N'Global') IS NULL EXEC(N'CREATE SCHEMA [Global];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    CREATE TABLE [Global].[Sessions] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [Password] nvarchar(450) NULL,
        [Status] int NOT NULL,
        [SessionMasterId] uniqueidentifier NULL,
        [Game] int NOT NULL,
        [GameSessionId] uniqueidentifier NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    CREATE TABLE [Global].[Players] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NULL,
        [PlayerNumber] int NOT NULL,
        [Name] nvarchar(max) NULL,
        [Status] int NOT NULL,
        CONSTRAINT [PK_Players] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Players_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Global].[Sessions] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    CREATE INDEX [IX_Players_SessionId] ON [Global].[Players] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    CREATE UNIQUE INDEX [IX_Sessions_Password] ON [Global].[Sessions] ([Password]) WHERE [Password] IS NOT NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    CREATE INDEX [IX_Sessions_SessionMasterId] ON [Global].[Sessions] ([SessionMasterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    ALTER TABLE [Global].[Sessions] ADD CONSTRAINT [FK_Sessions_Players_SessionMasterId] FOREIGN KEY ([SessionMasterId]) REFERENCES [Global].[Players] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200606093245_GlobalEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200606093245_GlobalEntities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201106234457_NullableGame')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Global].[Sessions]') AND [c].[name] = N'Game');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Global].[Sessions] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [Global].[Sessions] ALTER COLUMN [Game] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201106234457_NullableGame')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201106234457_NullableGame', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201109132510_emojis')
BEGIN
    ALTER TABLE [Global].[Players] ADD [Emoji] nvarchar(max) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201109132510_emojis')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201109132510_emojis', N'3.1.3');
END;

GO

