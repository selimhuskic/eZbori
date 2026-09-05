IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SavedSearches_Municipalities')
    ALTER TABLE [dbo].[SavedSearches]
    ADD CONSTRAINT [FK_SavedSearches_Municipalities] FOREIGN KEY ([MunicipalityCode]) REFERENCES [reference].[Municipalities] ([Id]);

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_ForecastedResults_Municipalities')
    ALTER TABLE [dbo].[ForecastedResults]
    ADD CONSTRAINT [FK_ForecastedResults_Municipalities] FOREIGN KEY ([MunicipalCode]) REFERENCES [reference].[Municipalities] ([Id]);
