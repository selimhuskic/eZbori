CREATE TABLE [elections].[StateMunicipalParties]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	[ElectionYear] INT NOT NULL,
	[MunicipalityCode] INT NOT NULL,
	[Code] NVARCHAR(512) NOT NULL,
	[ElectoralUnitPartyResultId] INT NOT NULL,
	[Name] NVARCHAR(512) NOT NULL,
	[Percentage] DECIMAL(18, 2) NOT NULL,
	[TotalVotes] INT NOT NULL
)
