ALTER TABLE [reference].[Municipalities]
ADD [CantonParliamentElectoralUnit] INT NULL

GO;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 201
WHERE [reference].[Municipalities].[Canton] = 1;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 202
WHERE [reference].[Municipalities].[Canton] = 2;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 203
WHERE [reference].[Municipalities].[Canton] = 3;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 204
WHERE [reference].[Municipalities].[Canton] = 4;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 205
WHERE [reference].[Municipalities].[Canton] = 5;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 206
WHERE [reference].[Municipalities].[Canton] = 6;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 207
WHERE [reference].[Municipalities].[Canton] = 7;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 208
WHERE [reference].[Municipalities].[Canton] = 8;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 209
WHERE [reference].[Municipalities].[Canton] = 9;

UPDATE [reference].[Municipalities]
SET [CantonParliamentElectoralUnit] = 210
WHERE [reference].[Municipalities].[Canton] = 10;