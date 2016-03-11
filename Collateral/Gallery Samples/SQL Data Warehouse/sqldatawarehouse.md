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


# Setting up a predictive analytics pipeline using Azure Data Fatory (ADF) and SQL Data Warehouse

## Use case

One paragraph

## Architecture

IaaS AWS/EC2 SQL Server to Azure Blob
![architecture-image1](./media/architecture.png)

## Prerequisites

- Microsoft Azure subscription with login credentials
- Microsoft SQL Server Management Studio

## Deploy

Get latest Azure PowerShell - insert link

    Login-AzureRmAccount
    Select-AzureRmSubscription -SubscriptionID "bc4170f0-cc6e-49d2-ba65-bc00a7a4df6b"
    New-AzureRmResourceGroup -Name rbaResourceGroup3 -Location "Central US"

### Using Local File

    New-AzureRmResourceGroupDeployment -Name rbaDeployment3 -ResourceGroupName rbaResourceGroup3 -TemplateFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount.json -TemplateParameterFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount-parameters.json

### Using URI

    New-AzureRmResourceGroupDeployment -Name rbaDeployment3 -ResourceGroupName rbaResourceGroup3 -TemplateUri https://raw.githubusercontent.com/roalexan/SolutionArchitects/10ce462255ba0199f7b06492bbf9fed9ed6d82ec/Collateral/Gallery%20Samples/SQL%20Data%20Warehouse/deploy-storageaccount.json -TemplateParameterFile C:\microsoft\sqldatawarehouse\arm\mine\deploy-storageaccount-parameters.json

## Undeploy

    Remove-AzureRmResourceGroup -Name rbaResourceGroup3

## Useful Links

https://azure.microsoft.com/en-us/documentation/templates/101-storage-account-create/
https://azure.microsoft.com/en-us/documentation/articles/resource-group-authoring-templates/
https://msdn.microsoft.com/en-us/library/mt603823.aspx
https://azure.microsoft.com/en-us/documentation/articles/powershell-azure-resource-manager/
https://blogs.technet.microsoft.com/dataplatforminsider/2014/07/30/transitioning-from-smp-to-mpp-the-why-and-the-how/
https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-overview-what-is/
