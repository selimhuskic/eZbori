CREATE TABLE [elections].[PresidencyMunicipalOverview]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
	[ElectionYear] INT NOT NULL,
	[Entity] INT NOT NULL,
	[TotalVoters] INT NOT NULL,
	[TotalVotes] INT NOT NULL,
	[TotalNoVotes] INT NOT NULL,
	[ValidVotes] INT NOT NULL,
	[TotalInvalidVotes] INT NOT NULL,
	[InvalidBlankBallots] INT NOT NULL,
	[InvalidOthersBallots] INT NOT NULL,
	[ProcessedPollingStationsPercentage] DECIMAL(18, 2) NOT NULL,
	[PercentageTotalVotes] DECIMAL(18, 2) NOT NULL,
	[PercentageTotalNoVotes] DECIMAL(18, 2) NOT NULL,
	[ProcessedTotalInvalidVotes] DECIMAL(18, 2) NOT NULL,
	[ProcessedInvalidBlankBallots] DECIMAL(18, 2) NOT NULL,
	[ProcessedInvalidOthersBallots] DECIMAL(18, 2) NOT NULL,
	[TotalPollingStations] INT NOT NULL,
	[ProcessedPollingStations] INT NOT NULL,
	[PartyNumber] INT NOT NULL,
	[CandidatesNumber] INT NOT NULL,
	[MunicipalityCode] INT NOT NULL
)
