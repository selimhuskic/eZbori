CREATE TABLE [elections].[EntityElectoralUnitParty]
(
	[Id] INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [ElectionYear] INT NOT NULL,
    [ElectoralUnitCode] INT NOT NULL,
    [AbsenceAndMobileTeamVotes] INT NOT NULL,
    [Code] NVARCHAR(512) NOT NULL,
    [CompensationMandates] INT NOT NULL,
    [ConfirmedVotes] INT NOT NULL,
    [ElectoralUnitParentPartyResultId] INT NOT NULL,
    [PartyName] NVARCHAR(512) NOT NULL,
    [Percentage] DECIMAL(18, 2) NOT NULL,
    [PostOfficeVotes] INT NOT NULL,
    [RegularMandates] INT NOT NULL,
    [RegularVotes] INT NOT NULL,
    [TotalMandates] INT NOT NULL,
    [TotalVotes] INT NOT NULL
)
