CREATE SCHEMA [elections]

CREATE TABLE [elections].[PresidencyResults]
(
	[Id] INT IDENTITY(1, 1) NOT NULL,
	[ElectionYear] INT NOT NULL,
	[CandidateName] NVARCHAR(512) NOT NULL,
	[Constituency] INT NOT NULL,
	[Code] NVARCHAR(512) NOT NULL,
	[TotalVotes] INT NOT NULL,
	[Percentage] DECIMAL(18, 2) NOT NULL,
	[AbsenceAndMobileTeamVotes] INT NOT NULL,
	[RegularVotes] INT NOT NULL,
	[MandateWon] BIT NOT NULL,
	[ConfirmedVotes] INT NOT NULL
	CONSTRAINT [Id] PRIMARY KEY CLUSTERED ([Id])
)
