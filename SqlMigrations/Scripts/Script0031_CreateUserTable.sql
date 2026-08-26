CREATE TABLE [dbo].[Users]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [Email] NVARCHAR(512) NOT NULL,
    [UserName] NVARCHAR(512) NOT NULL,
    [FirstName] NVARCHAR(512) NOT NULL,
    [LastName] NVARCHAR(512) NOT NULL,
    [DateOfBirth] DATETIME2 NULL,
    [Password] NVARCHAR(512) NOT NULL,
    [UserRole] INT NOT NULL,
    [UserVerified] BIT NOT NULL DEFAULT 0,
    [CreatedAt] DATETIME2 NOT NULL
);
GO


CREATE TABLE [dbo].[RefreshTokens]
(
    [Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [UserId] INT NOT NULL,
    [Token] VARCHAR(512) NOT NULL,
    [CreatedAt] DATETIME NOT NULL,
    [ExpiryDate] DATETIME,

    CONSTRAINT [FK_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id])
);
GO

CREATE NONCLUSTERED INDEX [IX_RefreshTokens_Token]
ON [dbo].[RefreshTokens] ([Token]);
