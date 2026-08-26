-- Unique email and username per user
ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [UQ_Users_Email] UNIQUE ([Email]);

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [UQ_Users_UserName] UNIQUE ([UserName]);

-- Unique role name
ALTER TABLE [dbo].[UserRoles]
ADD CONSTRAINT [UQ_UserRoles_RoleName] UNIQUE ([RoleName]);
