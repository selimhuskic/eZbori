IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.Notifications'))
BEGIN
    CREATE TABLE [dbo].[Notifications]
    (
        [Id]        INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
        [UserId]    INT NOT NULL,
        [Title]     NVARCHAR(200) NOT NULL,
        [Body]      NVARCHAR(1000) NOT NULL,
        [CreatedAt] DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        [IsRead]    BIT NOT NULL DEFAULT 0,
        CONSTRAINT [FK_Notifications_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE
    )
END
