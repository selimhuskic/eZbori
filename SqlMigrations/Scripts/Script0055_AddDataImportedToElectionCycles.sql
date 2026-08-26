IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.ElectionCycles') AND name = 'DataImported'
)
BEGIN
    ALTER TABLE dbo.ElectionCycles ADD DataImported BIT NOT NULL DEFAULT 0;
    EXEC sp_executesql N'
        UPDATE dbo.ElectionCycles SET DataImported = 1
        WHERE Year IN (2018, 2022) AND ElectionType = 1;
        UPDATE dbo.ElectionCycles SET DataImported = 1
        WHERE Year IN (2016, 2020) AND ElectionType = 2;
    ';
END
