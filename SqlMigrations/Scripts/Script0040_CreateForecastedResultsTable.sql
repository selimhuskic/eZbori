CREATE TABLE [dbo].[ForecastedResults]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [MunicipalCode] SMALLINT NULL,
    [CantonCode] SMALLINT NULL,
    [EntityCode] SMALLINT NULL,
    [IsStateCouncil] BIT NOT NULL,
    [ForecastedNumberOfVotes] FLOAT NULL,
    [PartyName] NVARCHAR(512) NOT NULL
)
