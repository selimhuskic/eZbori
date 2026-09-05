IF NOT EXISTS (SELECT 1 FROM sys.tables WHERE object_id = OBJECT_ID('dbo.ImportJobs'))
BEGIN
    CREATE TABLE dbo.ImportJobs (
        Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
        ElectionType TINYINT NOT NULL,
        Year SMALLINT NOT NULL,
        Status INT NOT NULL DEFAULT 0,
        ErrorMessage NVARCHAR(MAX) NULL,
        CreatedAt DATETIME2 NOT NULL,
        UpdatedAt DATETIME2 NOT NULL
    );
END
