IF EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_SavedSearches_Users')
BEGIN
    ALTER TABLE [dbo].[SavedSearches] DROP CONSTRAINT [FK_SavedSearches_Users];
END

ALTER TABLE [dbo].[SavedSearches]
    ADD CONSTRAINT [FK_SavedSearches_Users] FOREIGN KEY ([UserId]) REFERENCES [dbo].[Users] ([Id]) ON DELETE CASCADE;
