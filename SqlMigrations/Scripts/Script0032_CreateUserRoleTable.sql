CREATE TABLE [dbo].[UserRoles]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [RoleName] NVARCHAR(256) NOT NULL
)


ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_UserRoles] FOREIGN KEY ([UserRole]) REFERENCES [dbo].[UserRoles] ([Id]);