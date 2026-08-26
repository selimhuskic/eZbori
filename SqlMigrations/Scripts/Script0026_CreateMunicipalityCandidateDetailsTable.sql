CREATE TABLE [elections].[MunicipalityCandidateDetails]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [ElectionYear] INT NOT NULL,
    [MunicipalityCode] INT NOT NULL,
    [AbsenceAndMobileTeamVotes] INT NOT NULL,
    [Code] NVARCHAR(512) NOT NULL,
    [ConfirmedVotes] INT NOT NULL,
    [HaveMandates] BIT NOT NULL,
    [Name] NVARCHAR(512) NOT NULL,
    [Percentage] DECIMAL(18, 2) NOT NULL,
    [PostOfficeVotes] INT NOT NULL,
    [RegularVotes] INT NOT NULL,
    [TotalVotes] INT NOT NULL
)
