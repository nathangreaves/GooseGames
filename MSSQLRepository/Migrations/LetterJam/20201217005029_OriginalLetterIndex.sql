IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    IF SCHEMA_ID(N'LetterJam') IS NULL EXEC(N'CREATE SCHEMA [LetterJam];');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE TABLE [LetterJam].[LetterCards] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [Letter] nvarchar(1) NOT NULL,
        [PlayerId] uniqueidentifier NULL,
        [NonPlayerCharacterId] uniqueidentifier NULL,
        [LetterIndex] int NULL,
        [PlayerLetterGuess] nvarchar(1) NULL,
        [BonusLetter] bit NOT NULL,
        CONSTRAINT [PK_LetterCards] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE TABLE [LetterJam].[NonPlayerCharacters] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [Emoji] nvarchar(max) NULL,
        [Name] nvarchar(max) NULL,
        [PlayerNumber] int NOT NULL,
        [NumberOfLettersRemaining] int NOT NULL,
        [CurrentLetterId] uniqueidentifier NULL,
        [ClueReleased] bit NOT NULL,
        CONSTRAINT [PK_NonPlayerCharacters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_NonPlayerCharacters_LetterCards_CurrentLetterId] FOREIGN KEY ([CurrentLetterId]) REFERENCES [LetterJam].[LetterCards] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE TABLE [LetterJam].[PlayerStates] (
        [Id] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [Status] uniqueidentifier NULL,
        [StatusDescription] nvarchar(max) NULL,
        [CurrentLetterId] uniqueidentifier NULL,
        [CurrentLetterIndex] int NULL,
        [NumberOfCluesGiven] int NOT NULL,
        [OriginalWordLength] int NOT NULL,
        [FinalWordLength] int NULL,
        [Successful] bit NULL,
        [Points] int NULL,
        CONSTRAINT [PK_PlayerStates] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_PlayerStates_LetterCards_CurrentLetterId] FOREIGN KEY ([CurrentLetterId]) REFERENCES [LetterJam].[LetterCards] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE TABLE [LetterJam].[Rounds] (
        [Id] uniqueidentifier NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [RoundNumber] int NOT NULL,
        [RoundStatus] int NOT NULL,
        [ClueGiverPlayerId] uniqueidentifier NULL,
        [ClueId] uniqueidentifier NULL,
        CONSTRAINT [PK_Rounds] PRIMARY KEY ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE TABLE [LetterJam].[Games] (
        [Id] uniqueidentifier NOT NULL,
        [SessionId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [RedCluesRemaining] int NOT NULL,
        [GreenCluesRemaining] int NOT NULL,
        [LockedCluesRemaining] int NOT NULL,
        [GameStatus] int NOT NULL,
        [CurrentRoundId] uniqueidentifier NULL,
        [Points] int NULL,
        CONSTRAINT [PK_Games] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Games_Rounds_CurrentRoundId] FOREIGN KEY ([CurrentRoundId]) REFERENCES [LetterJam].[Rounds] ([Id]) ON DELETE NO ACTION
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_Games_CurrentRoundId] ON [LetterJam].[Games] ([CurrentRoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_Games_SessionId] ON [LetterJam].[Games] ([SessionId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_LetterCards_GameId] ON [LetterJam].[LetterCards] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_NonPlayerCharacters_CurrentLetterId] ON [LetterJam].[NonPlayerCharacters] ([CurrentLetterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_NonPlayerCharacters_GameId] ON [LetterJam].[NonPlayerCharacters] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_CurrentLetterId] ON [LetterJam].[PlayerStates] ([CurrentLetterId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_GameId] ON [LetterJam].[PlayerStates] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_PlayerStates_PlayerId] ON [LetterJam].[PlayerStates] ([PlayerId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    CREATE INDEX [IX_Rounds_GameId] ON [LetterJam].[Rounds] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    ALTER TABLE [LetterJam].[LetterCards] ADD CONSTRAINT [FK_LetterCards_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [LetterJam].[Games] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    ALTER TABLE [LetterJam].[NonPlayerCharacters] ADD CONSTRAINT [FK_NonPlayerCharacters_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [LetterJam].[Games] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    ALTER TABLE [LetterJam].[PlayerStates] ADD CONSTRAINT [FK_PlayerStates_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [LetterJam].[Games] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    ALTER TABLE [LetterJam].[Rounds] ADD CONSTRAINT [FK_Rounds_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [LetterJam].[Games] ([Id]) ON DELETE CASCADE;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201111225019_LetterJamInitialEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201111225019_LetterJamInitialEntities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE TABLE [LetterJam].[Clues] (
        [Id] uniqueidentifier NOT NULL,
        [RoundId] uniqueidentifier NOT NULL,
        [RoundNumber] int NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [ClueGiverPlayerId] uniqueidentifier NOT NULL,
        [ClueSuccessful] bit NOT NULL,
        [NumberOfLetters] int NOT NULL,
        [NumberOfPlayerLetters] int NOT NULL,
        [NumberOfNonPlayerLetters] int NOT NULL,
        [WildcardUsed] bit NOT NULL,
        [NumberOfBonusLetters] int NOT NULL,
        CONSTRAINT [PK_Clues] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clues_Rounds_RoundId] FOREIGN KEY ([RoundId]) REFERENCES [LetterJam].[Rounds] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE TABLE [LetterJam].[ClueLetters] (
        [Id] uniqueidentifier NOT NULL,
        [ClueId] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [PlayerId] uniqueidentifier NULL,
        [NonPlayerCharacterId] uniqueidentifier NULL,
        [LetterId] uniqueidentifier NULL,
        [Letter] nvarchar(1) NULL,
        [BonusLetter] bit NOT NULL,
        CONSTRAINT [PK_ClueLetters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClueLetters_Clues_ClueId] FOREIGN KEY ([ClueId]) REFERENCES [LetterJam].[Clues] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE TABLE [LetterJam].[ClueVotes] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [ClueId] uniqueidentifier NOT NULL,
        [RoundId] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        CONSTRAINT [PK_ClueVotes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ClueVotes_Clues_ClueId] FOREIGN KEY ([ClueId]) REFERENCES [LetterJam].[Clues] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ClueVotes_Rounds_RoundId] FOREIGN KEY ([RoundId]) REFERENCES [LetterJam].[Rounds] ([Id])
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE INDEX [IX_ClueLetters_ClueId] ON [LetterJam].[ClueLetters] ([ClueId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE INDEX [IX_Clues_RoundId] ON [LetterJam].[Clues] ([RoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE INDEX [IX_ClueVotes_ClueId] ON [LetterJam].[ClueVotes] ([ClueId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    CREATE INDEX [IX_ClueVotes_RoundId] ON [LetterJam].[ClueVotes] ([RoundId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201116164247_ClueEntities')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201116164247_ClueEntities', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201119131341_ClueLetterExtraColumns')
BEGIN
    ALTER TABLE [LetterJam].[ClueLetters] ADD [IsWildCard] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201119131341_ClueLetterExtraColumns')
BEGIN
    ALTER TABLE [LetterJam].[ClueLetters] ADD [LetterIndex] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201119131341_ClueLetterExtraColumns')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201119131341_ClueLetterExtraColumns', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201203220128_GameNumberOfPlayers')
BEGIN
    ALTER TABLE [LetterJam].[Games] ADD [NumberOfPlayers] int NOT NULL DEFAULT 0;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201203220128_GameNumberOfPlayers')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201203220128_GameNumberOfPlayers', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208103341_CardDiscarded')
BEGIN
    ALTER TABLE [LetterJam].[LetterCards] ADD [Discarded] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208103341_CardDiscarded')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201208103341_CardDiscarded', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    ALTER TABLE [LetterJam].[ClueLetters] ADD [LetterCardId] uniqueidentifier NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    Update LetterJam.ClueLetters set LetterCardId = LetterId where 1 = 1;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    DECLARE @var0 sysname;
    SELECT @var0 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LetterJam].[ClueLetters]') AND [c].[name] = N'LetterId');
    IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [LetterJam].[ClueLetters] DROP CONSTRAINT [' + @var0 + '];');
    ALTER TABLE [LetterJam].[ClueLetters] DROP COLUMN [LetterId];
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    CREATE INDEX [IX_ClueLetters_LetterCardId] ON [LetterJam].[ClueLetters] ([LetterCardId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    ALTER TABLE [LetterJam].[ClueLetters] ADD CONSTRAINT [FK_ClueLetters_LetterCards_LetterCardId] FOREIGN KEY ([LetterCardId]) REFERENCES [LetterJam].[LetterCards] ([Id]) ON DELETE NO ACTION;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201208230250_ClueLetterCardRelationship')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201208230250_ClueLetterCardRelationship', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201213131927_FinalWordLetters')
BEGIN
    CREATE TABLE [LetterJam].[FinalWordLetters] (
        [Id] uniqueidentifier NOT NULL,
        [CreatedUtc] datetime2 NOT NULL,
        [LastUpdatedUtc] datetime2 NOT NULL,
        [GameId] uniqueidentifier NOT NULL,
        [PlayerId] uniqueidentifier NOT NULL,
        [CardId] uniqueidentifier NULL,
        [LetterIndex] int NOT NULL,
        [Letter] nvarchar(1) NOT NULL,
        [PlayerLetterGuess] nvarchar(1) NULL,
        [BonusLetter] bit NOT NULL,
        [Wildcard] bit NOT NULL,
        CONSTRAINT [PK_FinalWordLetters] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_FinalWordLetters_Games_GameId] FOREIGN KEY ([GameId]) REFERENCES [LetterJam].[Games] ([Id]) ON DELETE CASCADE
    );
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201213131927_FinalWordLetters')
BEGIN
    CREATE INDEX [IX_FinalWordLetters_GameId] ON [LetterJam].[FinalWordLetters] ([GameId]);
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201213131927_FinalWordLetters')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201213131927_FinalWordLetters', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201215222955_FinalWordNullableLetter')
BEGIN
    DECLARE @var1 sysname;
    SELECT @var1 = [d].[name]
    FROM [sys].[default_constraints] [d]
    INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
    WHERE ([d].[parent_object_id] = OBJECT_ID(N'[LetterJam].[FinalWordLetters]') AND [c].[name] = N'Letter');
    IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [LetterJam].[FinalWordLetters] DROP CONSTRAINT [' + @var1 + '];');
    ALTER TABLE [LetterJam].[FinalWordLetters] ALTER COLUMN [Letter] nvarchar(1) NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201215222955_FinalWordNullableLetter')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201215222955_FinalWordNullableLetter', N'3.1.3');
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201217005029_OriginalLetterIndex')
BEGIN
    ALTER TABLE [LetterJam].[LetterCards] ADD [OriginalLetterIndex] int NULL;
END;

GO

IF NOT EXISTS(SELECT * FROM [__EFMigrationsHistory] WHERE [MigrationId] = N'20201217005029_OriginalLetterIndex')
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20201217005029_OriginalLetterIndex', N'3.1.3');
END;

GO

