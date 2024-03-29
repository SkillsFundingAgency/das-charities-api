﻿CREATE TABLE [dbo].[Charity]
(
    [Id] INT NOT NULL CONSTRAINT PK_Charity PRIMARY KEY, 
    [Name] VARCHAR(MAX) NOT NULL, 
    [CompaniesHouseNumber] VARCHAR(MAX) NULL, 
    [RegistrationNumber] INT NOT NULL, 
    [LinkedCharityId] INT NOT NULL, 
    [RegistrationDate] DATETIME2 NOT NULL, 
    [RegistrationStatus] INT NOT NULL, 
    [Type] INT NULL, 
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
