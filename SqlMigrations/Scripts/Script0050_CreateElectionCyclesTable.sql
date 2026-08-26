CREATE TABLE [dbo].[ElectionCycles]
(
    [Id]           INT IDENTITY(1, 1) PRIMARY KEY NOT NULL,
    [Year]         SMALLINT NOT NULL,
    [ElectionType] TINYINT NOT NULL,
    [ApiBaseUrl]   NVARCHAR(512) NOT NULL,
    [ResultKey]    NVARCHAR(128) NOT NULL,
    CONSTRAINT [UQ_ElectionCycles_Year_Type] UNIQUE ([Year], [ElectionType])
)

-- Seed the known cycles that are already in the system
INSERT INTO [dbo].[ElectionCycles] ([Year], [ElectionType], [ApiBaseUrl], [ResultKey])
VALUES
    (2018, 1, 'https://www.izbori.ba/api_2018', 'WebResult_2018GEN_2018_10_4_15_40_5'),
    (2022, 1, 'https://www.izbori.ba/api_2018', 'WebResult_2022GENT1_2022_4_20_14_10_43'),
    (2016, 2, 'https://www.izbori.ba/api_2018', 'WebResult_2016MUNI_2016_9_23_16_38_25'),
    (2020, 2, 'https://www.izbori.ba/api_2018', 'WebResult_2020MUNI_2020_11_10_15_0_18'),
    (2024, 2, 'https://www.izbori.ba/api_2018', 'WebResult_2024MUNI_2024_9_19_20_28_13')
