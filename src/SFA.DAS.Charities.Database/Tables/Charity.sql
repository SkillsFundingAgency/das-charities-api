CREATE TABLE [dbo].[Charity]
(
    [Id] INT NOT NULL PRIMARY KEY, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [CompanyNumber] VARCHAR(MAX) NOT NULL, 
    [RegisteredCharityNumber] INT NOT NULL, 
    [LinkedCharityId] INT NOT NULL, 
    [RegistrationDate] DATETIME2 NOT NULL, 
    [RegistrationStatus] TINYINT NOT NULL, 
    [Type] TINYINT NULL, 
    [FinancialPeriodStartDate] DATETIME2 NULL, 
    [FinancialPeriodEndDate] DATETIME2 NULL, 
    [Address1] VARCHAR(MAX) NULL, 
    [Address2] VARCHAR(MAX) NULL, 
    [Address3] VARCHAR(MAX) NULL, 
    [Address4] VARCHAR(MAX) NULL, 
    [Address5] VARCHAR(MAX) NULL, 
    [Postcode] VARCHAR(8) NULL, 
    [IsInsolvent] BIT NULL, 
    [IsInAdministration] BIT NULL, 
    [WasPreviouslyExcepted] BIT NULL, 
    [RemovalDate] DATETIME2 NULL, 
    [LastUpdatedDate] DATETIME2 NOT NULL DEFAULT GetUtcDate()
)
