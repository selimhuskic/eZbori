ALTER TABLE [elections].[PresidencyMunicipalOverview]
ADD CONSTRAINT [FK_PresidencyMunicipalOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[PresidencyMunicipalResults]
ADD CONSTRAINT [FK_PresidencyMunicipalResults_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[StateMunicipalOverview]
ADD CONSTRAINT [FK_StateMunicipalOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[StateMunicipalParties]
ADD CONSTRAINT [FK_StateMunicipalParties_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[CantonMunicipalOverview]
ADD CONSTRAINT [FK_CantonMunicipalOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[CantonMunicipalParties]
ADD CONSTRAINT [FK_CantonMunicipalParties_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[EntityMunicipalOverview]
ADD CONSTRAINT [FK_EntityMunicipalOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[EntityMunicipalParty]
ADD CONSTRAINT [FK_EntityMunicipalParty_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[EntityPresidentMunicipalCandidate]
ADD CONSTRAINT [FK_EntityPresidentMunicipalCandidate_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[MunicipalityCouncilOverview]
ADD CONSTRAINT [FK_MunicipalityCouncilOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[MunicipalityCouncilParty]
ADD CONSTRAINT [FK_MunicipalityCouncilParty_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[MunicipalityCouncilMinority]
ADD CONSTRAINT [FK_MunicipalityCouncilMinority_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[MunicipalityCandidateOverview]
ADD CONSTRAINT [FK_MunicipalityCandidateOverview_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

ALTER TABLE [elections].[MunicipalityCandidateDetails]
ADD CONSTRAINT [FK_MunicipalityCandidateDetails_Municipalities]
FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);
