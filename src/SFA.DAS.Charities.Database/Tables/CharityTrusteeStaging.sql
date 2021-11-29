CREATE TABLE [dbo].[CharityTrusteeStaging]
(
    [Id] INT NOT NULL IDENTITY(1,1) CONSTRAINT PK_CharityTrusteeStaging PRIMARY KEY,
    [CharityId] INT NOT NULL, 
    [RegistrationNumber] INT NOT NULL,
    [TrusteeId] INT NOT NULL, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [IsChair] BIT NOT NULL,
    [TrusteeType] TINYINT NOT NULL,
    [AppointmentDate] DATETIME2 NULL,
    [LastUpdatedDate] DATETIME2 NOT NULL DEFAULT GetUtcDate()
)

GO

CREATE NONCLUSTERED INDEX [IX_CharityTrusteeStaging_CharityId] ON [dbo].[CharityTrusteeStaging] ([CharityId])
