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

## Azure SQL Data Warehouse

Microsoft has been hard at work creating a set of compelling technologies to allow easy and affordable cloud services. Collectively known as Azure, this cloud computing platform is used for building, deploying, and managing applications and services through a global network of Microsoft-managed data centers. The relational database, especially Microsoft Azure SQL Server, is one of the key services offered on this stack. And now, based on the proven relational database engine of Microsoft Azure SQL Server, Microsoft offers an enterprise data warehouse: Microsoft SQL Data Warehouse.   

Microsoft SQL Data Warehouse is an elastic petabyte-scale, data warehouse-as-a-service offering. It can grow and shrink in minutes, works in Azure or on-premise, and uses T-SQL. Because it separates compute and storage, users only pay for the queries they need. It supports full indexing, partitions and columnar indexing. It is integrated with PowerBI for visualizing data, Azure Machine Learning for predictions, Azure Data Factory for event processing, and Azure HDInsight Hadoop-based big data analytics service.

![architecture-image1](./media/dwarchitecture.png)






elastically scale up and down
connect to Microsoft's Azure Machine Learning libraries
connects to Azure Data Lake (which integrates with Hadoop systems from Cloudera and Hortonworks)
	while support Microsoft's Hadoop tooling such as Azure HDInsight

the landing page in gallery should talk about:
1) the usecase and why we have solved it using SQL DW.
2) the contents of the git repo
3) success criteria.

The blog should talk about:
1) the industry (somewhat in the lines details of the text Jack’s original document under use case)
2) cloud / Microsoft, 3) what and why? (maybe uses the architecture diagram), 4) business impact.

## Use Case

![architecture-image1](./media/architecture2.png)

## Prerequisites

- Microsoft Azure subscription with login credentials
- PowerBI subscription with login credentials
- SQL client (Example: Microsoft SQL Server Management Studio)

## Deploy

### Resources

Many of the resources (SQL Server V12, SQL Warehouse, Service Bus, Event Hub, Stream Analytics Job) are deployed automatically when you click the **Deploy to Azure button**.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

Screenshots on how to set parameters.

<!--
<a href="http://armviz.io/#/?load=https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploy.json" target="_blank">
    <img src="http://armviz.io/visualizebutton.png"/>
</a>
-->

### Create Azure SQL Data Warehouse tables

1. Connect to the Data Warehouse using a SQL client of your choice. For example:
   1. Start: **Microsoft SQL Server Management Studio**
   1. Click: **File** > **Connect Object Explorer...**
   1. Select: Server type: **Database Engine**
   1. Type: Server name: **personal-[*UNIQUE*].database.windows.net**
   1. Select: Authentication: **SQL Server Authentication**
   1. Type: Login: **personaluser**
   1. Type: Password: **pass@word1**
   1. Check: **Remember password** # Optional
  1. Click: **Connect**
1. Create the tables. For example:
	 1. Expand: **personal-[*UNIQUE*].database.windows.net** > Databases > **personalDB**
	 1. Click: **New Query** # You may safely ignore the warning concerning QueryGovernorCostLimit if you see it
	 1. Copy and Paste:
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
            CREATE TABLE AverageRatings (
	           EventId INT,
	           AverageRating FLOAT
            )
            WITH (
	           CLUSTERED COLUMNSTORE INDEX
            )
     1. Click: **Execute**

### Edit and start the ASA job

1. Browse: https://manage.windowsazure.com
1. Click: **personalstreamanalytics<unique>**
1. Click: **OUTPUTS**
1. Click: **ADD OUTPUT**
1. Select: **Power BI**
1. Click: **Next**
1. Click: **Authorize Now** # Login with your credentials
1. Type: OUTPUT ALIAS: **OutputPowerBI**
1. Type: DATASET NAME: **personalDB** # This dataset will be overwritten in PBI should it already exist
1. Type: TABLE NAME: **personalDB**
1. Select: WORKSPACE: **My Workspace** # Default
1. Click: **Finish**
1. Click: **Start**
1. Click: **Finish** # You do not need to specify a custom time

### Download and run the data generator

1. Download **debug.zip**
1. Unzip
1. Edit: **Rage.exe.config**
1. Replace: EVENTHUBNAME: With: **personaleventhub[*UNIQUE*]**
1. Browse: https://manage.windowsazure.com # Get the endpoint
1. Click: SERVICE BUS
1. Click: CONNECTION INFORMATION
1. Copy: CONNECTION STRING
1. Replace: ENDPOINT: With: CONNECTION STRING
1. Double click: **Rage.exe** # Runs until you click any key on the console

#### Verify data being written

##### From Portal

1. Browse: https://manage.windowsazure.com
1. Click: **personalstreamanalytics[*UNIQUE*]**
1. Click: DASHBOARD
1. Click: **Operation Logs**
1. Select: a recent log
1. Click: DETAILS

##### From SQL Client

1. Connect to the Data Warehouse using a SQL client of your choice
1. Run SQL to view the latest entries. For example:
   1. Expand: **personal-[*UNIQUE*].database.windows.net** > Databases > **personalDB**
   1. Click: **New Query** # You may safely ignore the warning concerning QueryGovernorCostLimit if you see it
   1. Copy and Paste:
	      select * from Ratings order by DateTime desc;
   1. Click: **Execute**

### Create the PBI dashboard

1. Browse: https://powerbi.microsoft.com
1. Click: **Sign in** # Login with your credentials
1. Show: The navigation pane
1. Click: **personalDB** # Under the Datasets folder
1. Click: **Line chart** # Under Visualizations
1. Drag: **datetime**: To: **Axis**
1. Drag: **deviceid**: To: **Legend**
1. Drag: **rating**: To: **Values**
1. Click: **Save**
1. Type: Name: **personalDB**
1. Click: **Save**
1. Click: **Pin visual** # pin icon on upper-right
1. Select: **New dashboard**
1. Type: Name: **personalDB**
1. Click: **Pin**

### Create the AML service

1. Browse: https://studio.azureml.net
1. Click: **Sign** In # Login with your credentials
1. Click: Experiments > NEW
1. Click: Blank Experiment
1. Expand: Data Input and Output
1. Drag: Reader: To: Canvas
1. Select: Data source: Azure SQL Database
1. Type: Database server name: **personal-[*UNIQUE*].database.windows.net**
1. Type: Database name: personalDB
1. Type: Server user account name: personaluser
1. Type: Server user account password: pass@word1
1. Uncheck: Accept any server certificate (insecure): No # Default
1. Type: Database query:
       SELECT
       CAST(Rating AS INT) AS Rating
       FROM Ratings
       WHERE EventId = 1
1. Click: Database server name: web service parameter
1. Click: Database name: web service parameter
1. Click: Server user account name: web service parameter
1. Click: Server user account password: web service parameter
1. Click: Database query: web service parameter
1. Expand: Statistical Functions
1. Drag: Compute Elementary Statistics: To: Canvas
1. Select: Method: Mean
1. Connect: Reader: With: Compute Elementary Statistics
1. Expand: Web Service
1. Drag: Output: To: Canvas
1. Connect: Compute Elementary Statistics: Web service output
1. Click: RUN
1. Right Click: Compute Elementary Statistics
1. Select: Visualize # Verify that the mean output is reasonable
1. Click: Close
1. Click: SAVE AS
1. Type: Experiment name: Ratings
1. Click: SET UP WEB SERVICE
1. Click: NEXT > NEXT > NEXT > FINISH
1. Click: RUN
1. Click: DEPLOY WEB SERVICE
1. Click: TEST # Verify that request/response works
1. Click: DATABASE QUERY:
       SELECT
       CAST(Rating AS INT) AS Rating
       FROM Ratings
       WHERE EventId = 1
1. Click: DATABASE SERVER NAME: **personal-[*UNIQUE*].database.windows.net**
1. Click: DATABASE NAME: personalDB
1. Click: SERVER USER ACCOUNT NAME: personaluser
1. Click: SERVER USER ACCOUNT PASSWORD: pass@word1
1. Click: OK

### Create the ADF

Browse: https://portal.azure.com
Click: Data factories
Click: Add
Type: Name: datafactory-rba10
Select: Subscription: Boston Engineering
Select: Resource group name: rba10
Select: Region name: West US
Check: Pin to dashboard # The default
Click: Create
Click: Author and deploy

Click: New data store
Select: Azure SQL Data Warehouse
Replace: <servername>: With: personal-rba10
Replace: <databasename>: With: personalDB
Replace: <username>: With: personaluser
Replace: <servername>: With: personal-rba10
Replace: <password>: With: pass@word1
Click: Deploy

Browse: https://studio.azureml.net
Click: Sign In # Login with your credentials
Click: WEB SERVICES
Click: Ratings
Click: Copy # APIKEY
Click: BATCH EXECUTION
Copy: https://...jobs # BESURL
Click: New compute
Select: Azure ML
Replace: <Specify the batch scoring URL>: With: BESURL
Replace: <Specify the published workspace model's API key>: With: APIKEY
Remove: Line: Containing: updateResourceEndpoint
Remove: comma from preceding line
Click: Deploy
	
Click: New dataset
Select: Azure SQL Data Warehouse
Edit: linkedServiceName: AzureSqlDWLinkedService
Edit: tableName: Ratings
Edit: frequency: Minute
Edit: interval: 15 # Remove surround double quotes. 15 minutes is the minimum allowed.
Click: Deploy

Click: New dataset
Select: Azure SQL Data Warehouse
Edit: name: AzureSqlDWOutput
Edit: linkedServiceName: AzureSqlDWLinkedService
Edit: tableName: AverageRatings
Edit: frequency: Minute
Edit: interval: 15 # Remove surround double quotes. 15 minutes is the minimum allowed.
Click: Deploy

Click: More commands
Click: New pipeline
Edit:
{
    "name": "SQL-to-AML-to-SQL",
    "properties": {
        "description": "SQL to AML to SQL",
        "activities": [
            {
                "type": "AzureMLBatchScoring",
                "typeProperties": {
                    "webServiceParameters": {
						"Database server name": "personal-rba10.database.windows.net",
						"Database name": "personalDB",
						"Server user account name": "personaluser",
						"Server user account password": "pass@word1",
                        "Database query": "SELECT CAST(Rating AS INT) AS Rating FROM Ratings WHERE EventId = 1"
                    }
                },
                "inputs": [
                    {
                        "name": "AzureSqlDWInput"
                    }
                ],
                "outputs": [
                    {
                        "name": "AzureSqlDWOutput"
                    }
                ],
                "policy": {
                    "timeout": "00:05:00",
                    "concurrency": 1,
                    "retry": 3
                },
                "name": "MLActivity",
                "description": "prediction analysis on batch input",
                "linkedServiceName": "AzureMLLinkedService"
            }
        ],	
        "start": "2016-03-24T00:00:00Z",
        "end": "2016-03-25T00:00:00Z",
        "isPaused": false
    }
}
Click: Deploy

## Undeploy
1. Browse: https://portal.azure.com
1. Expand: Resource groups
1. Select: your resource group
1. Click: ...
1. Select: Delete
