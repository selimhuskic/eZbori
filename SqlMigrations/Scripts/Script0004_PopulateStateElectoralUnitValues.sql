--FBiH--

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 511
WHERE [reference].[Municipalities].[Entity] = 1 AND 
			[reference].[Municipalities].[Id] IN (1, 2, 3, 4, 5, 30, 32, 57, 59, 84, 85, 106, 107, 124)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 512
WHERE [reference].[Municipalities].[Entity] = 1 AND 
			[reference].[Municipalities].[Id] IN (125, 126, 127, 148, 149, 150, 171, 172, 173, 174, 176, 181, 199)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 513
WHERE [reference].[Municipalities].[Entity] = 1 AND 
			[reference].[Municipalities].[Id] IN (118, 130, 131, 133, 135, 136, 137, 139, 141, 143, 165, 167)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 514
WHERE [reference].[Municipalities].[Entity] = 1 AND 
			[reference].[Municipalities].[Id] IN (37, 39, 42, 65, 67, 75, 77, 89, 91, 93, 94, 95, 96, 109, 110, 111, 112, 113, 114, 115, 116, 117, 129, 183)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 515
WHERE [reference].[Municipalities].[Entity] = 1 AND 
			[reference].[Municipalities].[Id] IN (17, 20, 22, 25, 27, 36, 44, 47, 49, 50, 52, 55, 78, 79, 80, 82, 98)
GO;

--*RS--

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 521
WHERE [reference].[Municipalities].[Entity] = 2 AND 
			[reference].[Municipalities].[Id] IN (6, 7, 8, 9, 10, 11, 12, 13, 31, 33, 34, 35, 58, 61, 64, 66, 68, 70, 88, 108, 184)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 522
WHERE [reference].[Municipalities].[Entity] = 2 AND 
			[reference].[Municipalities].[Id] IN (14, 16, 18, 21, 23, 24, 26, 28, 29, 38, 40, 45, 54, 56, 74)
GO;

UPDATE [reference].[Municipalities]
SET [reference].[Municipalities].[StateParliamentElectoralUnit] = 523
WHERE [reference].[Municipalities].[Entity] = 2 AND 
			[reference].[Municipalities].[Id] IN (81, 83, 101, 103, 104, 105, 121, 123, 132, 138, 140, 142, 144, 146, 147, 158, 161, 163, 164,
			166, 168, 169, 170, 177, 179, 180, 182, 185)
GO;