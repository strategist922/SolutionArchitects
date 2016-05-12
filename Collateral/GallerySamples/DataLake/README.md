<properties
	pageTitle="Setting up predictive analytics pipelines using Azure SQL Data Warehouse | Microsoft Azure"
	description="Setting up predictive analytics pipelines using Azure SQL Data Warehouse."
	keywords="adf, azure data factory"
	services="sql-data-warehouse,data-factory,event-hubs,machine-learning,service-bus,stream-analytics"
	documentationCenter=""
	authors="daden"
	manager="paulettm"
	editor=""/>

<tags
	ms.service="sql-data-warehouse"
	ms.workload="data-services"
	ms.tgt_pltfrm="na"
	ms.devlang="na"
	ms.topic="article"
	ms.date="05/12/2016"
	ms.author="daden" />

# Setting up predictive analytics pipelines using Azure Data Lake

## Azure Data Lake

## Use Case

## Requirements

## Architecture

## Deploy

### Service Bus, Event Hub, Stream Analytics Job, ...

To get started, click the below button.

<a target="_blank" id="deploy-to-azure" href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2FCollateral%2FGallery%2FSamples%2FData%20Lake%2Fazuredeploypart1.json"><img src="http://azuredeploy.net/deploybutton.png"/></a>

This will create a new "blade" in the Azure portal.

![arm1-image](./media/arm1.png)

1. Parameters
   1. Type: UNIQUE (string): **[*UNIQUE*]** (You need to select a globally unique string)
   1. Select: LOCATION: **[*LOCATION*]** (The region where everything will be deployed)
   1. Click: **OK**
1. Select: Subscription: **[*SUBSCRIPTION*]** (The Azure subscription you want to use)
1. Resource group
   1. Select: **New**
   1. Type: New resource group name: **[*UNIQUE*]** (Same as above)
1. Select: Resource group location: **[*LOCATION*]** (Same as above)
1. Click: **Review legal terms** > **Create**
1. Check: **Pin to dashboard** (If you want it on your dashboard)
1. Click: **Create**

### Create the AML service

### Edit and start the ASA job

### Deploy the data generator as a Web Job

## Create the Data Factories

When you are ready, click this button

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploypart2.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

This will create a new "blade" in the Azure portal.

![arm1-image](./media/arm2.png)

1. Parameters
   1. Type: UNIQUE (string): **[*UNIQUE*]** (Use the one previously entered)
   1. Select: LOCATION: **[*LOCATION*]** (Use the one previously selected)
   1. Type: AZUREMLENDPOINT: **[*AZUREMLENDPOINT*]**
	    1. Browse: https://studio.azureml.net
	    1. Click: **WEB SERVICES** > **Ratings** > **BATCH EXECUTION**
	    1. Copy: POST: **REQUEST URI** (Everything from "https" up to and including "jobs")
   1. Type: AZUREMLAPIKEY: **[*AZUREMLAPIKEY*]**
	    1. Browse: https://studio.azureml.net
	    1. Click: **WEB SERVICES** > **Ratings**
	    1. Click: Copy: **API key**
   1. Click: **OK**
1. Select: Subscription: **[*SUBSCRIPTION*]** (Use the one previously selected)
1. Select: Resource group: **[*UNIQUE*]** (Use the one previously selected)
1. Click: **Review legal terms** > **Create**
1. Check: **Pin to dashboard** (If you want it on your dashboard)
1. Click: **Create**

## Start the predictive pipeline

## Create the PBI dashboard

## Summary

## Undeploy

## Debugging
