SET IDENTITY_INSERT [dbo].[UserRoles] ON;

IF NOT EXISTS (SELECT 1 FROM [dbo].[UserRoles] WHERE [Id] = 1)
    INSERT INTO [dbo].[UserRoles] ([Id], [RoleName]) VALUES (1, 'User');

IF NOT EXISTS (SELECT 1 FROM [dbo].[UserRoles] WHERE [Id] = 2)
    INSERT INTO [dbo].[UserRoles] ([Id], [RoleName]) VALUES (2, 'Administrator');

SET IDENTITY_INSERT [dbo].[UserRoles] OFF;
