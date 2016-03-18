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
- Microsoft SQL Server Management Studio

## Get Latest PowerShell

Download the Azure PowerShell module. Run Microsoft Web Platform Installer. http://go.microsoft.com/fwlink/p/?linkid=320376&clcid=0x409

## Deploy to Azure
<a href="https://azuredeploy.net/?repository=https://github.com/roalexan/SolutionArchitects" target="_blank"><img src="http://azuredeploy.net/deploybutton.png"/></a>
from MongoDB

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2FAzure%2Fazure-quickstart-templates%2Fmaster%2Fmongodb-high-availability%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

edited

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

* Login-AzureRmAccount
* Select-AzureRmSubscription -SubscriptionID "bc4170f0-cc6e-49d2-ba65-bc00a7a4df6b"
* New-AzureRmResourceGroup -Name rbaResourceGroup7 -Location "Central US"
* New-AzureRmResourceGroupDeployment -Name rbaDeployment3 -ResourceGroupName rbaResourceGroup3 -TemplateUri https://raw.githubusercontent.com/roalexan/SolutionArchitects/c7505f45a5a7985e58c9a52efad77085b47647a7/Collateral/Gallery%20Samples/SQL%20Data%20Warehouse/deploy.json

## Create tables

    CREATE TABLE Events (
    EventId INT,
    EventName VARCHAR(20) NOT NULL,
    Speakers VARCHAR(200) NOT NULL,
    Topics VARCHAR(200) NOT NULL,
    Country VARCHAR(10) NOT NULL,
    State VARCHAR(2) NOT NULL,
    City VARCHAR(20) NOT NULL
    )
    WITH (
    DISTRIBUTION = HASH(EventId),
    CLUSTERED COLUMNSTORE INDEX
    )

    CREATE TABLE Ratings (
    DateTime DATETIME2,
    EventId INT,
    Rating INT
    )
    WITH (
    DISTRIBUTION = HASH(DateTime),
    CLUSTERED COLUMNSTORE INDEX
    )

INSERT INTO Events VALUES(1234,'MLADS','roalexan,jacrowle','sqldw,dl','USA','WA','Redmond')




# JUNK FROM HERE DOWN

### Using Local File

    New-AzureRmResourceGroupDeployment -Name rbaDeployment3 -ResourceGroupName rbaResourceGroup3 -TemplateFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount.json -TemplateParameterFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount-parameters.json

### Using URI

    New-AzureRmResourceGroupDeployment -Name rbaDeployment3 -ResourceGroupName rbaResourceGroup3 -TemplateUri https://raw.githubusercontent.com/roalexan/SolutionArchitects/10ce462255ba0199f7b06492bbf9fed9ed6d82ec/Collateral/Gallery%20Samples/SQL%20Data%20Warehouse/deploy-storageaccount.json -TemplateParameterFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount-parameters.json

## Install local SQL Server
![sqlserverlogin-image1](./media/sqlserverlogin.png)
## Create database

Expand: **SERVERNAME**, right click: **Databases**: select: **New Database...**, type: Database name: **SQLSERVERDATABASENAME** # Up to user, click: **OK**

## Create table and populate with data

Expand: **Databases**, right click: **SQLSERVERDATABASENAME**: New Query

Copy below text and paste into query window

	IF OBJECT_ID('dbo.SystemEvents', 'U') IS NOT NULL DROP TABLE dbo.SystemEvents
	CREATE TABLE dbo.SystemEvents(DateTime DATETIME2(7) NOT NULL,ComputerName NVARCHAR(50) NOT NULL, EventID NVARCHAR(50),EventMessage NVARCHAR(500) CONSTRAINT PK_SystemEvents PRIMARY KEY CLUSTERED(DateTime,ComputerName))
	INSERT INTO dbo.SystemEvents VALUES('08/13/2015 00:15:07.262','MACHINE1','3','System Reboot')

Click: **Execute**

## Install Datamanagement gateway locally
## Create table in local SQL Server



## Undeploy

    Remove-AzureRmResourceGroup -Name rbaResourceGroup3

## Appendix create Datamanagement gateway

## Appendix small/large populate tables

## Useful Links

https://azure.microsoft.com/en-us/documentation/templates/101-storage-account-create/
https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/
https://msdn.microsoft.com/en-us/library/mt603823.aspx
https://azure.microsoft.com/en-us/documentation/articles/powershell-azure-resource-manager/
https://blogs.technet.microsoft.com/dataplatforminsider/2014/07/30/transitioning-from-smp-to-mpp-the-why-and-the-how/
https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-overview-what-is/
https://github.com/Azure/azure-quickstart-templates
https://azure.microsoft.com/en-us/documentation/articles/resource-manager-supported-services/
https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-get-started-provision-powershell/
https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-overview-develop/
