<properties
	pageTitle="ADF Data Movement from IaaS AWS SQLServer to Azure Blob and SQLServer | Microsoft Azure"
	description="Describes the steps needed to copy data from an IaaS AWS/EC2 SQLServer to Azure Blob and SQLServer."
	keywords="adf, azure data factory"
	services="datafactory"
	documentationCenter=""
	authors="roalexan"
	manager="paulettm"
	editor=""/>

<tags
	ms.service="datafactory"
	ms.workload="data-services"
	ms.tgt_pltfrm="na"
	ms.devlang="na"
	ms.topic="article"
	ms.date="02/23/2016"
	ms.author="roalexan" />

# Setting up predictive analytics pipelines using Azure SQL Data Warehouse

## Use case

Fill in  - one paragraph

## Architecture

![architecture-image1](./media/architecture.png)

## Prerequisites

- Microsoft Azure subscription with login credentials
- PowerBI subscription with login credentialas
- Microsoft SQL Server Management Studio

## Deploy to Azure

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>

## Create tables

Connect to the Data Warehouse using a SQL client of your choice
    Example:
		Start: Microsoft SQL Server Management Studio
		Click: File > Connect Object Explorer...
			Select: Server type: Database Engine
			Type: Server name: personal-<unique>.database.windows.net
			Select: Authentication: SQL Server Authentication
			Type: Login: personaluser
			Type: Password: pass@word1
			Check: Remember password # Up to user
			Click: Connect
Create the tables
    Example:
	    Expand: personal-<unique>.database.windows.net > Databases > personalDB
		Click: New Query # You may safely ignore the warning concerning QueryGovernorCostLimit if you see it
		Copy and Paste:

    CREATE TABLE Ratings (
    DateTime DATETIME2,
    EventId INT,
    Rating INT,
    DeviceId INT,
    Lat DECIMAL(8,5),
    Lon DECIMAL(8,5)
    )
    WITH (
    DISTRIBUTION = HASH(DateTime),
    CLUSTERED COLUMNSTORE INDEX
    )
	Click: Execute
		
## Edit and start the ASA job
	
Browse: https://manage.windowsazure.com
	
### Edit

Click: personalstreamanalytics<unique>
Click: OUTPUTS
Click: ADD OUTPUT
Select: Power BI
Click: Next
Click: Authorize Now # Login with your credentials
Type: OUTPUT ALIAS: OutputPowerBI
Type: DATASET NAME: personalDB # Note: This dataset will be overwritten in PBI, should it already exist
Type: TABLE NAME: **personalDB**
Select: WORKSPACE: My Workspace # Default
Click: Finish

### Start

Click: personalstreamanalytics<unique>
Click: Start
Click: Finish # You do not need to specify a custom time

## Run the data generator

Download data generator zip
Unzip
Edit: Rage.exe.config
Replace: EVENTHUBNAME: personaleventhub<unique>
Get the endpoint
    Browse: https://manage.windowsazure.com
	Click: SERVICE BUS
	Click: CONNECTION INFORMATION
	Copy: CONNECTION STRING
Replace: ENDPOINT: CONNECTION STRING
Double click: Rage.exe # Runs the executable

## Verify data being written

### From Portal

Browse: https://manage.windowsazure.com
Click: personalstreamanalytics<unique>
Click: DASHBOARD
Click: Operation Logs
Select: log # up to user
Click: DETAILS

### From SQL Client

Connect to the Data Warehouse using a SQL client of your choice
    Example:
		Start: Microsoft SQL Server Management Studio
		Click: File > Connect Object Explorer...
			Select: Server type: Database Engine
			Type: Server name: personal-<unique>.database.windows.net
			Select: Authentication: SQL Server Authentication
			Type: Login: personaluser
			Type: Password: pass@word1
			Check: Remember password # Up to user
			Click: Connect
Create the tables
    Example:
	    Expand: personal-<unique>.database.windows.net > Databases > personalDB
		Click: New Query # You may safely ignore the warning concerning QueryGovernorCostLimit if you see it
		Copy and Paste:
	select * from Ratings order by EventId, DateTime;
	Click: Execute

## Create the PBI dashboard

Browse: https://powerbi.microsoft.com
Click: Sign in # Login with your credentials
Show: The navigation pane
Click: personalDB # Under the Datasets folder
Click: Line chart # Under Visualizations
Drag: datetime > To: Axis
Drag: deviceid > To: Legend 
Drag: rating > To: Values

## Create the AML service

## Undeploy

Remove-AzureRmResourceGroup -Name rbaResourceGroup3