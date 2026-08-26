ALTER TABLE [dbo].[Users]
ADD [MunicipalityId] INT NULL;
GO

ALTER TABLE [dbo].[Users]
ADD [ProfileImageBase64] NVARCHAR(MAX) NULL;
GO

ALTER TABLE [dbo].[Users]
ADD CONSTRAINT [FK_Users_Municipalities]
FOREIGN KEY ([MunicipalityId]) REFERENCES [reference].[Municipalities] ([Id]);
GO
