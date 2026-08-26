CREATE TABLE [dbo].[SavedSearches]
(
    [Id]               INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [UserId]           INT NOT NULL,
    [ElectionType]     TINYINT NOT NULL,
    [ElectionYear]     SMALLINT NOT NULL,
    [ElectoralUnit]    INT NULL,
    [MunicipalityCode] INT NULL,
    [CreatedAt]        DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
    [IsDeleted]        BIT NOT NULL DEFAULT 0,
    CONSTRAINT [FK_SavedSearches_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
)
