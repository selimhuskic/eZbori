-- Presidency
CREATE NONCLUSTERED INDEX [IX_PresidencyResults_ElectionYear_Constituency]
    ON [elections].[PresidencyResults] ([ElectionYear], [Constituency]);

CREATE NONCLUSTERED INDEX [IX_PresidencyOverview_ElectionYear_Entity]
    ON [elections].[PresidencyOverview] ([ElectionYear], [Entity]);

CREATE NONCLUSTERED INDEX [IX_PresidencyMunicipalOverview_ElectionYear_MunicipalityCode]
    ON [elections].[PresidencyMunicipalOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_PresidencyMunicipalResults_ElectionYear_MunicipalityCode]
    ON [elections].[PresidencyMunicipalResults] ([ElectionYear], [MunicipalityCode]);

-- State
CREATE NONCLUSTERED INDEX [IX_StateElectoralUnitOverview_ElectionYear_ElectoralUnit]
    ON [elections].[StateElectoralUnitOverview] ([ElectionYear], [ElectoralUnit]);

CREATE NONCLUSTERED INDEX [IX_StateElectoralUnitParty_ElectionYear_ElectoralUnit]
    ON [elections].[StateElectoralUnitParty] ([ElectionYear], [ElectoralUnit]);

CREATE NONCLUSTERED INDEX [IX_StateMunicipalOverview_ElectionYear_MunicipalityCode]
    ON [elections].[StateMunicipalOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_StateMunicipalParty_ElectionYear_MunicipalityCode]
    ON [elections].[StateMunicipalParties] ([ElectionYear], [MunicipalityCode]);

-- Entity
CREATE NONCLUSTERED INDEX [IX_EntityElectoralUnitOverview_ElectionYear_ElectoralUnitCode]
    ON [elections].[EntityElectoralUnitOverview] ([ElectionYear], [ElectoralUnitCode]);

CREATE NONCLUSTERED INDEX [IX_EntityElectoralUnitParty_ElectionYear_ElectoralUnitCode]
    ON [elections].[EntityElectoralUnitParty] ([ElectionYear], [ElectoralUnitCode]);

CREATE NONCLUSTERED INDEX [IX_EntityPresidentOverview_ElectionYear_Entity]
    ON [elections].[EntityPresidentOverview] ([ElectionYear], [Entity]);

CREATE NONCLUSTERED INDEX [IX_EntityPresidentMunicipalCandidate_ElectionYear_MunicipalityCode]
    ON [elections].[EntityPresidentMunicipalCandidate] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_EntityMunicipalOverview_ElectionYear_MunicipalityCode]
    ON [elections].[EntityMunicipalOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_EntityMunicipalParty_ElectionYear_MunicipalityCode]
    ON [elections].[EntityMunicipalParty] ([ElectionYear], [MunicipalityCode]);

-- Canton
CREATE NONCLUSTERED INDEX [IX_CantonElectoralUnitOverview_ElectionYear_CantonElectoralUnitCode]
    ON [elections].[CantonElectoralUnitOverview] ([ElectionYear], [CantonElectoralUnitCode]);

CREATE NONCLUSTERED INDEX [IX_CantonElectoralUnitParty_ElectionYear_CantonElectoralUnitCode]
    ON [elections].[CantonElectoralUnitParty] ([ElectionYear], [CantonElectoralUnitCode]);

CREATE NONCLUSTERED INDEX [IX_CantonMunicipalOverview_ElectionYear_MunicipalityCode]
    ON [elections].[CantonMunicipalOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_CantonMunicipalParty_ElectionYear_MunicipalityCode]
    ON [elections].[CantonMunicipalParties] ([ElectionYear], [MunicipalityCode]);

-- Local elections (Municipality)
CREATE NONCLUSTERED INDEX [IX_MunicipalityCandidateDetails_ElectionYear_MunicipalityCode]
    ON [elections].[MunicipalityCandidateDetails] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_MunicipalityCandidateOverview_ElectionYear_MunicipalityCode]
    ON [elections].[MunicipalityCandidateOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_MunicipalityCouncilOverview_ElectionYear_MunicipalityCode]
    ON [elections].[MunicipalityCouncilOverview] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_MunicipalityCouncilParty_ElectionYear_MunicipalityCode]
    ON [elections].[MunicipalityCouncilParty] ([ElectionYear], [MunicipalityCode]);

CREATE NONCLUSTERED INDEX [IX_MunicipalityCouncilMinority_ElectionYear_MunicipalityCode]
    ON [elections].[MunicipalityCouncilMinority] ([ElectionYear], [MunicipalityCode]);
