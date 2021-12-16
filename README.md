## ⛔Never push sensitive information such as client id's, secrets or keys into repositories including in the README file⛔

# Charities API

<img src="https://avatars.githubusercontent.com/u/9841374?s=200&v=4" align="right" alt="UK Government logo">

[![Build Status](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_apis/build/status/das-charities-api?branchName=main)](https://dev.azure.com/sfa-gov-uk/Digital%20Apprenticeship%20Service/_build/latest?definitionId=2670&branchName=main)
[![Confluence Project](https://img.shields.io/badge/Confluence-Project-blue)](https://skillsfundingagency.atlassian.net/wiki/spaces/NDL/pages/3486253077/RoATP+-+Charities+API+Technical+Design)
[![License](https://img.shields.io/badge/license-MIT-lightgrey.svg?longCache=true&style=flat-square)](https://en.wikipedia.org/wiki/MIT_License)


## About
das-charities-api is an inner api for charities lookup. Here you can query a charity details like it's name, registration number, trustees etc. The data files are in json format and are uploaded to charity commissions website in individual compressed (zip) files. Since we cannot consume this as is, it was required that we create our own cache of this data and store in a structure format to be able to query it. 

## Import Functions
### How it works
In short, a timer triggered function which runs ones daily at 7pm from Monday to Friday, invokes durable function that executes following steps 
* Downloads the zip files and stores them in a blob storage.
* Extracts data from blob storage and uploads into staging tables.
* Refreshes data into live tables from staging tables. 

The details specification is documented on confluence page linked above. 

### Pre-Requisites
* A clone of this repository
* A code editor that supports Azure functions and .NetCore 3.1
* A storage account/emulator for blobs
* SQL Server instance for data

### Config
The functions app uses the standard Apprenticeship Service configuration. 

Alternatively you could configure the [SFA.DAS.Charities.Import](https://github.com/SkillsFundingAgency/das-charities-api) project as per its config file in [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-charities-api/SFA.DAS.Charities.Import.Functions.json)

### 🔗 External Dependencies
The import functions uses the [Charities commissions website](https://ccewuksprdoneregsadata1.blob.core.windows.net/data/json/) to import data from. 

## Charities API
There is one endpoint that takes charity registration number as argument, queries the SQL database and returns the details if found. 

### Pre-Requisites
* A clone of this repository
* A code editor that supports Azure functions and .NetCore 3.1
* SQL Server instance populated with charities data

### Config
Configure the [SFA.DAS.Charities.Api](https://github.com/SkillsFundingAgency/das-charities-api) project as per its config file in [das-employer-config](https://github.com/SkillsFundingAgency/das-employer-config/blob/master/das-charities-api/SFA.DAS.Charities.Api.json)


## Technologies
* .NetCore 3.1
* AspDotNetCore MVC Web API
* Azure Durable Functions V3
* SQL Service
* Azure Table Storage
* Azure Blob Storage
* NUnit
* Moq