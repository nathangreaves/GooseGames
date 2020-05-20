IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200508143804_Codenames')
BEGIN
    IF SCHEMA_ID(N'Codenames') IS NULL EXEC(N'CREATE SCHEMA [Codenames];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200508143804_Codenames')
BEGIN
    CREATE TABLE [Codenames].[Sessions] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [Password] nvarchar(max) NULL,
        [IsBlueTurn] bit NOT NULL,
        [BlueVictory] bit NULL,
        [SessionWordsId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_Sessions] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200508143804_Codenames')
BEGIN
    CREATE TABLE [Codenames].[Words] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [SessionWordsId] uniqueidentifier NOT NULL,
        [Word] nvarchar(max) NULL,
        [WordType] int NOT NULL,
        [Revealed] bit NOT NULL,
        [RevealedByBlue] bit NULL,
        CONSTRAINT [PK_Words] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Words_Sessions_SessionId] FOREIGN KEY ([SessionId]) REFERENCES [Codenames].[Sessions] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200508143804_Codenames')
BEGIN
    CREATE INDEX [IX_Words_SessionId] ON [Codenames].[Words] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20200508143804_Codenames')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20200508143804_Codenames', N'3.1.3');
END;

GO

