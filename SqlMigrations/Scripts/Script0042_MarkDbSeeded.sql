ALTER TABLE [elections].[StateMunicipalOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[StateMunicipalParties]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[PresidencyMunicipalResults]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[PresidencyMunicipalOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[MunicipalityCouncilParty]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[MunicipalityCouncilOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[MunicipalityCouncilMinority]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[MunicipalityCandidateOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[MunicipalityCandidateDetails]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[EntityPresidentMunicipalCandidate]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[EntityMunicipalParty]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[EntityMunicipalOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[CantonMunicipalParties]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

ALTER TABLE [elections].[CantonMunicipalOverview]
ADD FOREIGN KEY (MunicipalityCode) REFERENCES [reference].[Municipalities](Id);

