CREATE TABLE [dbo].[CharityTrustee]
(
    [Id] INT NOT NULL CONSTRAINT PK_CharityTrustee PRIMARY KEY,
    [CharityId] INT NOT NULL, 
    [RegisteredCharityNumber] INT NOT NULL,
    [TrusteeId] INT NOT NULL, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [IsChair] BIT NOT NULL,
    [TrusteeType] TINYINT NOT NULL,
    [AppointmentDate] DATETIME2 NULL,
    [LastUpdatedDate] DATETIME2 NOT NULL DEFAULT GetUtcDate()
)
