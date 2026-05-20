## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Charities API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-charities-api?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2670&branchName=main)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=SkillsFundingAgency_das-charities-api&metric=alert_status)](https://sonarcloud.io/summary/new_code?id=SkillsFundingAgency_das-charities-api)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About
das-charities-import is a function app to load charities organisation data from the charities commisssion into live charities database tables.

das-charities-api is an inner api for charities lookup. Here you can query a charity details like it's name, registration number, trustees etc. The data files are in json format and are uploaded to charity commissions website in individual compressed (zip) files. Since we cannot consume this as is, it was required that we create our own cache of this data and store in a structure format to be able to query it. 

## Import Functions
### How it works
A timer triggered function which runs ones daily at 7pm from Monday to Friday, invokes a function that executes following steps 
* Downloads the zip files.
* Extracts the data and uploads into staging tables.
* Refreshes data into live tables from staging tables. 

### Pre-Requisites
* A clone of this repository
* Visual Studio or similar IDE
* .NET 10 
* A storage emulator (for example Azurite)
* SQL Server instance for data

### Config
Configure the SFA.DAS.Charities.Import project as per its config file in [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-charities-api/SFA.DAS.Charities.Import.Functions.json)

In the `SFA.DAS.Charities.Import.Jobs` project, if not exist already, add local.settings.json file with following content:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConfigNames": "SFA.DAS.Charities.Import.Functions",
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
    "EnvironmentName": "LOCAL",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
    "CharitiesDataImportTimerInterval": "0 0 19 * * 1-5"
  }
}
```

### 🔗 External Dependencies
The import functions uses the [Charities commissions website](https://ccewuksprdoneregsadata1.blob.core.windows.net/data/json/) to import data from. 

## Charities API
There is one endpoint that takes charity registration number as argument, queries the SQL database and returns the details if found. 

### Pre-Requisites
* A clone of this repository
* Visual Studio or similar IDE
* .NET 10 
* A storage emulator (for example Azurite)
* SQL Server instance populated with charities data

### Config
Configure the SFA.DAS.Charities.Api project as per its config file in [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-charities-api/SFA.DAS.Charities.Api.json)

In the `SFA.DAS.Charities.Api` project, if not exist already, add local.settings.json file with following content:
```
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "ConfigNames": "SFA.DAS.Charities.Api",
    "ConfigurationStorageConnectionString": "UseDevelopmentStorage=true",
    "EnvironmentName": "LOCAL",
    "APPLICATIONINSIGHTS_CONNECTION_STRING": "",
    "SqlConnectionString": "<SQL_CONNECTION_STRING>"
  }
}
```

## Technologies
* .NetCore 10.0
* AspDotNetCore MVC Web API
* SQL Service
* Azure Table Storage
* NUnit
* Moq
