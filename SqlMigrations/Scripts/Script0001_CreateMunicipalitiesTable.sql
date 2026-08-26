CREATE SCHEMA [reference]

CREATE TABLE [reference].[Municipalities]
(
	[Id] INT NOT NULL,
	[Name] NVARCHAR(255) NOT NULL,
	[Canton] INT NULL,
	[Entity] INT NULL,
	[District] BIT NULL,
	CONSTRAINT [Id] PRIMARY KEY CLUSTERED ([Id])
)
