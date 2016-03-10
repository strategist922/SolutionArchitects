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


# Learn how to copy data from IaaS AWS SQLServer to Azure blob and SQLServer using Azure Data Factory

## Description

This article shows you step-by-step how to use Azure Data Factory (ADF) to ingest data from an Amazon IaaS SQL Server (one in which it is deployed to AWS/EC2) to Azure blob storage and Azure SQL Server. This is achieved by creating an ADF with two pipelines. You may choose to create either one or both.

IaaS AWS/EC2 SQL Server to Azure Blob
![description-image1](./media/iaas_aws_sqlserver_to_azure/description-image1.png)

IaaS AWS/EC2 SQL Server to Azure SQL Server
![description-image2](./media/iaas_aws_sqlserver_to_azure/description-image2.png)

## Prerequisites

- Microsoft Azure subscription with login credentials
- AWS Account with login credentials
- SQL Server deployed with database and table to AWS (see appendix)
- Microsoft SQL Server Management Studio
- Azure Storage Explorer

## Create Data Factory on Azure

Browse: https://portal.azure.com, select: **Data factories**, click: **Add**, type: Name: **ADFNAME** # Up to user, select: Subscription: **AZURESUBSCRIPTIONNAME** # Up to user, click: Resource group name: **Or create new**, type: Resource group name: **ADFRESOURCEGROUPNAME** # Up to user, region name: **West US** # Up to user, click: **Create**

### Create Data Management Gateway for Data Factory

Click: **Author and deploy** # Wait until ready, right click: **Data Gateways**, select: **New data gateway**, type: Data gateway name: **ADFDATAGATEWAYNAME** # Up to user, click: **OK**

![adf-new-key](./media/iaas_aws_sqlserver_to_azure/adf-new-key.png)

Click: **Copy**: # This is the **ADFDATAGATEWAYKEY**. Copy this somewhere. This is used to register the Data Management Gateway on the EC2 instance.

## Deploy Data Management Gateway on AWS/EC2 instance

### Login to AWS/EC2 instance

Right Click: **EC2KEYPAIRFILEPATH** # File path of EC2 key pair file, open with **Remote Desktop Connection**, click: **Connect**, click: **Use another account**, type: User name: **Administrator**, type: password: **EC2KEYPAIRPASSWORD**, click: **OK**

### Disable Internet Explorer Enhanced Security Configuration (IE ESC)

Click: **Windows button**, click: **Server Manager**, click: **Local Server**, click: IE Enhanced Security Configuration: **On**, select: Administrators: **Off**, select: Users: **Off**

### Install Data Management Gateway (on AWS/EC2 instance)

Click: **Windows button**, click: **Internet Explorer**, browse: http://www.microsoft.com/en-us/download/details.aspx?id=39717 # If needed, search for **Data Management Gateway** at https://www.microsoft.com/en-us/download, click: **Download**, select: **DataManagementGateway_*_en-us (64-bit).msi** # Use latest version, click: **Next**, click: **Save** # default download directory: **C:\Downloads\DataManagementGateway_*_en-us (64-bit)**, click: **Run**, click: **Next**, check: **I accept…**, click: **Next** # Default installation directory C:\Program Files\Microsoft Data Management Gateway\, click: **Install** # Wait until done, click: **Finish**, paste: **ADFDATAGATEWAYKEY** # Previously copied when creating new Data Gateway, click: **Show gateway key** # Up to user, click: **Enabled verbose logging for troubleshooting purposes** # Up to user, click: **Register**

![configuration-manager](./media/iaas_aws_sqlserver_to_azure/configuration-manager.png)

Click: **Export**, type: Password: **EC2DATAGATEWAYCERTIFICATEPASSWORD** # Up to user, click: **Next**

NOTE: Events are found by default at: Event Viewer>Applications and Services Logs>Data Management Gateway

### Test Data Management Gateway (on EC2)

Double click: **C:\Program Files\Microsoft Data Management Gateway\1.0\Shared\ConfigManager** # Default installation directory, click: **Diagnostics**, select: Data source type: **SqlServer**, type: Server: **SERVERNAME**, type: Database: **SQLSERVERDATABASENAME**, select: Authentication mode: **Database**, type: User name: **SQLSERVERSQLLOGINNAME**, type: Password: **SQLSERVERSQLPASSWORD**, click: **Test connection**

## Create Pipeline To Azure Blob Storage

### Create Azure storage account

Browse: https://portal.azure.com, click: **Azure Portal**, click: **Storage**, click: **NEW**, click: **QUICK CREATE**, type: URL: **AZURESTORAGEACCOUNT** # Up to user. Must be unique, type: Location: **East US** # Up to user, click: **CREATE STORAGE ACCOUNT**, click: **MANAGE ACCESS KEYS**, copy: **SECONDARY ACCESS KEY**

### Create Azure storage container

Click: **AZURESTORAGEACCOUNT**, click: **Containers**, click: **CREATE A CONTAINER**, name: **AZURESTORAGECONTAINER** # Up to user, select: Access: **Private**, click: **OK**

### Modify ADF to import data from SQL Server (AWS/EC2) to Azure storage container

Click: **Data factories**, select: **ADFNAME**, click: **Author and deploy**

### Add SQL Server Linked Service

Click: **New data store**, select: **SQL Server**

	{
		"name": "SqlServerLinkedService",
		"properties": {
			"type": "OnPremisesSqlServer",
			"description": "",
			"typeProperties": {
				"connectionString": "Data Source=SERVERNAME;Initial Catalog=SQLSERVERDATABASENAME;Integrated Security=False;User ID=SQLSERVERSQLLOGINNAME;Password=SQLSERVERSQLPASSWORD;",
				"gatewayName": "AZUREDATAFACTORYDATAGATEWAYNAME"
			}
		}
	}

Click: **Deploy**

### Add Azure Storage Linked Service

Click: **New data store**, select: **Azure storage**

	{
		"name": "AzureStorageLinkedService",
		"properties": {
			"description": "",
			"type": "AzureStorage",
			"typeProperties": {
				"connectionString": "DefaultEndpointsProtocol=https;AccountName=AZURESTORAGEACCOUNT;AccountKey=AZURESTORAGEACCOUNTKEY"
			}
		}
	}

Click: **Deploy**

### Add SQL Server Dataset

Click: **New dataset**, select: **SQL Server table**

	{
		"name": "SQLServerDataset",
		"properties": {
			"type": "SqlServerTable",
			"linkedServiceName": "SqlServerLinkedService",
			"typeProperties": {
				"tableName": "SystemEvents"
			},
			"availability": {
				"frequency": "Day",
				"interval": 1
			},
			"external": true,
			"policy": {}
		}
	}

Click: **Deploy**

### Add Azure Blob Dataset

Click: **New dataset**, select: **Azure Blob storage**

	{
		"name": "AzureBlobDataset",
		"properties": {
			"type": "AzureBlob",
			"linkedServiceName": "AzureStorageLinkedService",
			"typeProperties": {
				"folderPath": "AZURESTORAGECONTAINER/myblobfolder/",
				"format": {
					"type": "TextFormat",
					"columnDelimiter": ","
				}
			},
			"availability": {
				"frequency": "Day",
				"interval": 1
			}
		}
	}

Click: **Deploy**

### Add pipeline

Click: **New pipeline** # You may need first click **More commands** to see the button. Also, be aware that this pipeline is intended for one-time data load, so the SQL has no time where condition.

	{
		"name": "SQLServerToAzureBlobPipeline",
		"properties": {
			"description": "pipeline for copy activity",
			"activities": [
				{
					"type": "Copy",
					"typeProperties": {
						"source": {
							"type": "SqlSource",
							"sqlReaderQuery": "select * from SystemEvents"
						},
						"sink": {
							"type": "BlobSink",
							"writeBatchSize": 0,
							"writeBatchTimeout": "00:00:00"
						}
					},
					"inputs": [
					{
						"name": "SQLServerDataset"
					}
					],
					"outputs": [
					{
						"name": "AzureBlobDataset"
					}
					],
					"policy": {
						"timeout": "01:00:00",
						"concurrency": 1,
						"executionPriorityOrder": "NewestFirst",
						"style": "StartOfInterval",
						"retry": 0
					},
					"scheduler": {
						"frequency": "Day",
						"interval": 1
					},
					"name": "SQLServerToBlob",
					"description": "copy activity"
				}
			],
			"start": "2015-11-05T00:00:00Z",
			"end": "2015-11-06T00:00:00Z",
			"isPaused": false
		}
	}

Click: **Deploy**

### Verify pipeline works

Select: **ADFNAME**, select: **Diagram**. You should see a diagram as below.

![blob-pipeline](./media/iaas_aws_sqlserver_to_azure/blob-pipeline.png)

Select: **ADFNAME**, select: **Dataset**, select: **SQLServerDataset**. You should see something close to the below.

![blob-status](./media/iaas_aws_sqlserver_to_azure/blob-status.png)

Select: **ADFNAME**, select: **Dataset**, select: **AzureBlobDataset**

As a final check Select: **ADFNAME**, select: **Dataset**, select: **AzureBlobDataset**. You should see the blob in the container. For example, here’s what you see using Azure Storage Explorer:

![azure-storage-explorer](./media/iaas_aws_sqlserver_to_azure/azure-storage-explorer.png)

## Create Pipeline To Azure SQL Server

### Create Azure SQL Server

Browse: https://portal.azure.com, click: **New**, Select: **Data + Storage**, Select: **SQL Database**, Type: **AZURESQLSERVERDATABASENAME** # Up to user, click: **Create a new server**, type: Server name: **AZURESQLSERVERSERVERNAME** # Up to user, type: Server admin login: **AZURESQLSERVERSERVERADMINLOGIN** # Up to user, type: Password: **AZURESQLSERVERSERVERPASSWORD** # Up to user, select: Location: **South Central US** # Up to user,  select: Create V12 server (Latest update): **Yes**, check: Allow azure services to access server: **true**, click: **OK**, Select: Select source: **Blank database**, Select: Pricing tier: **Standard S0**, Select: Optional configuration: **Collation**, Select: Resource group: **Create a new resource group**, Type:  **AZURERESOURCEGROUPNAME**, click: **OK**, click: **Create**

### Create database and table in Azure SQL Server

#### Configure firewall

Browse: https://portal.azure.com, click: **SQL Databases**, click: **AZURESQLSERVERDATABASENAME**, click: Server name: **AZURESQLSERVERSERVERNAME**,  click: **Show firewall settings**, browse: http://www.whatsmyip.org/ # Get **YOURIPADDRESS** - your client’s IP address, paste: RULE NAME: **YOURIPADDRESS**, paste: START IP: **YOURIPADDRESS**, paste: END IP: **YOURIPADDRESS**, click: **Save**

#### Get connection string

Browse: https://portal.azure.com, click: **SQL Databases**, click: **AZURESQLSERVERDATABASENAME**, click: **Show database connection strings**, Copy: Between: **Server=** and **;Database=** # Example: **AZURESQLSERVERSERVERNAME.database.windows.net, 1433**

Click: **SQL Server Management Studio**

![management-studio](./media/iaas_aws_sqlserver_to_azure/management-studio.png)

Type: Server name: **AZURESQLSERVERSERVERNAME.database.windows.net, 1433**, Select: Authentication: **SQL Server Authentication**, type: Login: **AZURESQLSERVERSERVERADMINLOGIN**, type: Password: **AZURESQLSERVERSERVERPASSWORD**,  click: **Connect**

#### Create table and populate with data

Expand: **Databases**, right click: AZURESQLSERVERDATABASENAME: **New Query**

Copy below text and paste into query window

	IF OBJECT_ID('dbo.SystemEvents', 'U') IS NOT NULL DROP TABLE dbo.SystemEvents
	CREATE TABLE dbo.SystemEvents(DateTime DATETIME2(7) NOT NULL,ComputerName NVARCHAR(50) NOT NULL, EventID NVARCHAR(50),EventMessage NVARCHAR(500) CONSTRAINT PK_SystemEvents PRIMARY KEY CLUSTERED(DateTime,ComputerName))

#### Modify ADF to import data from IaaS SQL Server (AWS/EC2) to Azure SQL Server

Click: **Data factories**, select: **ADFNAME**, click: **Author and deploy**

#### Add Azure SQL Server Linked Service

Click: **New dataset**, select: **Azure SQL**

	{
		"name": "ToSqlServerLinkedService",
		"properties": {
			"type": "AzureSqlDatabase",
			"description": "",
			"typeProperties": {
				"connectionString": "Data Source=tcp:AZURESQLSERVERSERVERNAME.database.windows.net,1433;Initial Catalog=SQLSERVERDATABASENAME;User ID= AZURESQLSERVERSERVERADMINLOGIN @ AZURESQLSERVERSERVERNAME;Password= AZURESQLSERVERSERVERPASSWORD;Integrated Security=False;Encrypt=True;Connect Timeout=30"
			}
		}
	}

#### Add Azure SQL Server Dataset

Click: **New dataset**, select: **Azure SQL**

	{
		"name": "ToSQLServerDataset",
		"properties": {
			"type": "AzureSqlTable",
			"linkedServiceName": "ToSqlServerLinkedService",
			"typeProperties": {
				"tableName": "SystemEvents"
			},
			"availability": {
				"frequency": "Day",
				"interval": 1
			}
		}
	}

#### Add Pipeline

Click: **New pipeline** # You may need first click **More commands** to see the button. Also, be aware that this pipeline is intended for one-time data load, so the SQL has no time where condition.

	{
		"name": "OnPremSQLToAzureSQLPipeline",
		"properties": {
			"description": "On Prem SQL to Azure SQL",
			"activities": [
				{
					"type": "Copy",
					"typeProperties": {
						"source": {
							"type": "SqlSource",
							"sqlReaderQuery": "select * from SystemEvents"
						},
						"sink": {
							"type": "SqlSink",
							"writeBatchSize": 1000,
							"writeBatchTimeout": "00:30:00"
						}
					},
					"inputs": [
						{
							"name": "FromSQLServerDataset"
						}
					],
					"outputs": [
						{
							"name": "ToSQLServerDataset"
						}
					],
					"policy": {
						"timeout": "01:00:00",
						"concurrency": 1,
							"executionPriorityOrder": "NewestFirst",
							"style": "StartOfInterval",
							"retry": 0
					},
					"scheduler": {
						"frequency": "Day",
						"interval": 1
					},
					"name": "Activity-OnPremSQLToAzureSQL"
				}
			],
			"start": "2015-11-05T00:00:00Z",
			"end": "2015-11-06T00:00:00Z",
			"isPaused": false
		}
	}

#### Verify pipeline works

Select: **ADFNAME**, select: **Diagram**. You should see a diagram as below.

![sql-pipeline](./media/iaas_aws_sqlserver_to_azure/sql-pipeline.png)

Select: **ADFNAME**, select: **Dataset**, select: **ToSQLServerDataset**. You should see something close to the below.

![sql-status](./media/iaas_aws_sqlserver_to_azure/sql-status.png)

As a final check, you should see the rows in your destination database table. For example, you should see something like this using SQL Server Management Studio:

![sql-studio](./media/iaas_aws_sqlserver_to_azure/sql-studio.png)

## Appendices

### Deploy SQL Server on AWS/EC2

If you already have a SQL Server on AWS/EC2 you intend to use for this article, you can skip this section.

Login: **AWS console**, select: Region: **US East (N. Virginia)** # Up to user, click: **EC2**, click: **Launch Instance**, click: select: **Microsoft Windows Server 2012 R2 with SQL Server Express** # Use latest AMI, check: **t2.large** # up to user, click: **Next: Configure Instance Details**, click: **Next: Add Storage**, click: **Next: Tag Instance**, type: value: **EC2INSTANCENAME** # Up to user, click: **Next: Configure Security Group**, select: **Create a new security group**, type: Security group name: **EC2SECURITYGROUPNAME** # Up to user, browse: http://www.whatsmyip.org/ # Get **YOURIPADDRESS** - your client’s IP address, select: MS SQL: Source: Custom IP: type: **YOURIPADDRESS/32** # Example: 111.222.333.444/32, select: RDP, Source: Custom IP: type: **YOURIPADDRESS/32** # Example: 111.222.333.444/32, click: **Review and Launch**, click: **Launch**, select: **Create a new key pair**, type: Key pair name: **EC2KEYPAIRNAME** # Up to user, click: **Download Key Pair** to **EC2KEYPAIRFILEPATH** # Up to user, click: **Launch Instances** # Instance ID is displayed, click: **View Instances**, check: **EC2INSTANCENAME**. Wait until the instance state is **running**, click: **Connect**, click: **Get Password**, copy: file contents: **EC2KEYPAIRFILEPATH**, click: **Decrypt Password** # Make note as **EC2KEYPAIRPASSWORD**, click: **Download Remote Desktop File**, click: **Close**

### Create database and table in SQL Server on AWS/EC2 instance

#### Login to AWS/EC2 instance

Right Click: **EC2KEYPAIRFILEPATH** # File path of EC2 key pair file, open with **Remote Desktop Connection**, click: **Connect**, click: **Use another account**, type: User name: **Administrator**, type: password: **EC2KEYPAIRPASSWORD**, click: **OK**

#### Login to SQL Server

Click: **Windows button**, click: **SQL Server 2014 Management Studio**

![management-studio](./media/iaas_aws_sqlserver_to_azure/management-studio.png)

Select: **Server name, <Browse for more…>**, open: **Database Engine**, select: **SERVERNAME**, click: **OK**, click: **Connect**

![connect-to-server](./media/iaas_aws_sqlserver_to_azure/connect-to-server.png)

#### Create database

Expand: **SERVERNAME**, right click: **Databases**: select: **New Database...**, type: Database name: **SQLSERVERDATABASENAME** # Up to user, click: **OK**

![new-database](./media/iaas_aws_sqlserver_to_azure/new-database.png)

#### Create table and populate with data

Expand: **Databases**, right click: **SQLSERVERDATABASENAME**: New Query

Copy below text and paste into query window

	IF OBJECT_ID('dbo.SystemEvents', 'U') IS NOT NULL DROP TABLE dbo.SystemEvents
	CREATE TABLE dbo.SystemEvents(DateTime DATETIME2(7) NOT NULL,ComputerName NVARCHAR(50) NOT NULL, EventID NVARCHAR(50),EventMessage NVARCHAR(500) CONSTRAINT PK_SystemEvents PRIMARY KEY CLUSTERED(DateTime,ComputerName))
	INSERT INTO dbo.SystemEvents VALUES('08/13/2015 00:15:07.262','MACHINE1','3','System Reboot')

Click: **Execute**

#### Create new user and connect with SQL Authentication

Expand: **SERVERNAME**, select: **Security**, right click: **Logins**, select: **New Login…**

![new-login](./media/iaas_aws_sqlserver_to_azure/new-login.png)

Select: **SQL Server authentication**, type: Login name: **SQLSERVERSQLLOGINNAME** # Up to user, type: Password: **SQLSERVERSQLPASSWORD** # Up to user, click: **OK**

![server-properties](./media/iaas_aws_sqlserver_to_azure/server-properties.png)

Right click: **SERVERNAME**, select: **Properties**, select: **Security**, select: **SQL Server and Windows Authentication mode**, click: **OK**

![login-properties](./media/iaas_aws_sqlserver_to_azure/login-properties.png)

Expand: **SERVERNAME**, select: **Security**, select: **Logins**, right click: **SQLSERVERSQLLOGINNAME**, click: **Properties**, select: **User Mapping**, check: All databases, click: **OK**, double click: **SQLSERVERSQLLOGINNAME**, click: **Securables**, check: Grant: **Select All User Securables**, click: **OK**

#### Restart SQL Server

Click: **Windows button**, type: **Services**, click: **Services**

![services](./media/iaas_aws_sqlserver_to_azure/services.png)

Select: **SQL Server (MSSQLSERVER)**, click: **Restart the service**

![services-list](./media/iaas_aws_sqlserver_to_azure/services-list.png)

#### Test SQL Login

Click: **Windows button**, click: **SQL Server 2014 Management Studio**

![management-studio](./media/iaas_aws_sqlserver_to_azure/management-studio.png)

Select: Authentication: **SQL Server Authentication**, Type: Login: **SQLSERVERSQLLOGINNAME**, type: Password: **SQLSERVERSQLPASSWORD**

![connect-to-server](./media/iaas_aws_sqlserver_to_azure/connect-to-server.png)

**NOTE**: You may be required to create a new user password.

Expand: **SERVERNAME**, expand: **Databases**, expand: **SQLSERVERDATABASENAME**, expand: **Tables**

You should see dbo.SystemEvents
