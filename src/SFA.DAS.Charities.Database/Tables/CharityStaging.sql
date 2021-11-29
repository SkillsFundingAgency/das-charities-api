CREATE TABLE [dbo].[CharityStaging]
(
    [Id] INT NOT NULL CONSTRAINT PK_CharityStaging PRIMARY KEY, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [CompaniesHouseNumber] VARCHAR(MAX) NULL, 
    [RegistrationNumber] INT NOT NULL, 
    [LinkedCharityId] INT NOT NULL, 
    [RegistrationDate] DATETIME2 NOT NULL, 
    [RegistrationStatus] TINYINT NOT NULL, 
    [Type] TINYINT NULL, 
    [FinancialPeriodStartDate] DATETIME2 NULL, 
    [FinancialPeriodEndDate] DATETIME2 NULL, 
    [AddressLine1] VARCHAR(MAX) NULL, 
    [AddressLine2] VARCHAR(MAX) NULL, 
    [AddressLine3] VARCHAR(MAX) NULL, 
    [AddressLine4] VARCHAR(MAX) NULL, 
    [AddressLine5] VARCHAR(MAX) NULL, 
    [Postcode] VARCHAR(8) NULL, 
    [IsInsolvent] BIT NULL, 
    [IsInAdministration] BIT NULL, 
    [WasPreviouslyExcepted] BIT NULL, 
    [RemovalDate] DATETIME2 NULL, 
    [LastUpdatedDate] DATETIME2 NOT NULL DEFAULT GetUtcDate()
)
