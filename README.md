<properties
	pageTitle="Setting up predictive analytics pipelines using Azure SQL Data Warehouse | Microsoft Azure"
	description="Setting up predictive analytics pipelines using Azure SQL Data Warehouse."
	keywords="adf, azure data factory"
	services="sql-data-warehouse,data-factory,event-hubs,machine-learning,service-bus,stream-analytics"
	documentationCenter=""
	authors="roalexan"
	manager="paulettm"
	editor=""/>

<tags
	ms.service="sql-data-warehouse"
	ms.workload="data-services"
	ms.tgt_pltfrm="na"
	ms.devlang="na"
	ms.topic="article"
	ms.date="04/18/2016"
	ms.author="roalexan" />

# Setting up predictive analytics pipelines using Azure SQL Data Warehouse

## Azure SQL Data Warehouse

 <a href="https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-overview-what-is"/>Azure SQL Data Warehouse</a> is a cloud-based, scalable database capable of processing massive volumes of data. It is ideally suited for storing structured data with a defined schema. Due to it's <a href="https://technet.microsoft.com/en-us/library/hh393582%28v=sql.110%29.aspx"/>MPP</a> architecture and use of Azure storage, it provides optimized query performance along with the ability to grow or shrink storage and compute independently. SQL Data Warehouse uses SQL Server's Transact-SQL (<a href="https://msdn.microsoft.com/en-us/library/mt243830.aspx"/>TSQL</a>) syntax for many operations and supports a broad set of traditional SQL constructs such as stored procedures, user-defined functions, table partitioning, indexes, and collations. It is integrated with traditional SQL Server and third party tools, along with many services in Azure such as <a href="https://azure.microsoft.com/en-us/services/data-factory/"/>Azure Data Factory</a>, <a href="https://azure.microsoft.com/en-us/services/stream-analytics/"/>Stream Analytics</a>, <a href="https://azure.microsoft.com/en-us/services/machine-learning/">Machine Learning</a>, and <a href="https://powerbi.microsoft.com/en-us/"/>Power BI</a>.

## Use Case

To demonstrate the power of Azure SQL Data Warehouse we will examine a sample use case that integrates SQL Data Warehouse with Azure Machine Learning. Along the way you will also be exposed to a number of other Azure components, namely Event Hub, Stream Analytics, and Power BI - as well as an on-prem SQL Server via a Data Management Gateway. At the end of this gallery we will include the steps to deploy this use case in your Azure subscription.

The use case is a rating system that allows users to rate an event (such as a conference talk) and visualize the results in real time (5 second intervals). Ratings are also stored in a data warehouse and sent to machine learning for near real time predictions (15 minute intervals). Lastly, historical ratings are bulk-loaded from an on-prem database.

When everything is successfully deployed and running, the final result will be a PowerBI dashboard showing the ratings of each individual device in real time and the average rating for all devices.

Here is a screenshot of a sample dashboard.

![dashboard-usecase-image](./media/dashboard-realtime-and-predictive.png)

## Requirements

- Microsoft <a href="https://azure.microsoft.com/en-us/">Azure</a> subscription with login credentials
- <a href="https://powerbi.microsoft.com/">PowerBI</a> subscription with login credentials
- An environment that will host your "on-prem" database (your laptop, a virtual machine) with
    - SQL Server
    - <a href="https://msdn.microsoft.com/en-us/library/dn879362.aspx">Data Management Gateway</a>
- A local installation of <a href="https://azure.microsoft.com/en-us/documentation/articles/sql-data-warehouse-install-visual-studio/">Visual Studio with SQL Server Data Tools (SSDT)</a>

## Architecture

The architecture is illustrated below.

![architecture-usecase-image](./media/architecture-usecase.png)

Real time ratings are generated via a data generator deployed as an <a href="https://azure.microsoft.com/en-us/documentation/articles/web-sites-create-web-jobs/">Azure Web Job</a>. New ratings are generated every 5 seconds for four simulated devices (and one event) and sent to an Event Hub. An Azure Stream Analytics job then sends the ratings to a) Power BI for real time visualization and b) SQL Data Warehouse for storage and predictive analytics

Predictive analytics is done by sending the ratings from SQL Data Warehouse to the batch endpoint of an experiment published as a web service in the Azure Machine Learning Studio, where the results are written back to SQL Data Warehouse. An Azure Data Factory orchestrates these operations. For now the experiment simply returns the average rating for the event, but you could consider doing more advanced analytics such as computing the average rating in Stream Analytics and doing predictions in AML based on location, event type, and device counts.

Bulk-loading is done by creating the schema in an on-prem SQL Server and populating the table with sample ratings data (downloadable from GitHub). A Data Management Gateway is used in the on-prem environment to connect the SQL Server to Azure SQL Data Warehouse. A Data Factory schedules the copying of the data from on-prem SQL Server to SQL Data Warehouse.

## Deploy

Below are the steps to deploy the use case into your Azure subscription. Note that to condense the steps somewhat, a **>** is used between repeated actions. For example:

1. Click: **Button A**
1. Click: **Button B**

is written as

1. Click: **Button A** > **Button B**  

### Service Bus, Event Hub, Stream Analytics Job, SQL Server, and SQL Data Warehouse

To get started, click the below button.

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploypart1.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

This will create a new "blade" in the Azure portal.

![arm1-image](./media/arm1.png)

1. Parameters
   1. Type: UNIQUE (string): **[*UNIQUE*]** # Select a globally unique string
   1. Select: LOCATION: **[*LOCATION*]** # The region where everything will be deployed
   1. Click: **OK**
1. Select: Subscription: **[*SUBSCRIPTION*]** # Azure subscription you want to use
1. Resource group
   1. Select: **New**
   1. Type: New resource group name: **[*UNIQUE*]** # Same as above
1. Select: Resource group location: **[*LOCATION*]** # Same as above
1. Click: **Review legal terms** > **Create**
1. Check: **Pin to dashboard** # If you want it on your dashboard
1. Click: **Create**

### Create and populate on-prem SQL Server tables

1. Connect to the on-prem SQL Server using a SQL client of your choice (such as SQL Server Management Studio)
1. Download sample ratings: https://github.com/Azure/Cortana-Intelligence-Gallery-Content/blob/master/Tutorials/SQL-Data-Warehouse/historical-ratings.csv
1. Download sample average ratings:
https://github.com/Azure/Cortana-Intelligence-Gallery-Content/blob/master/Tutorials/SQL-Data-Warehouse/historical-averageratings.csv
1. Load sample data. For example, by using the SQL Server Import and Export Wizard in the SQL Server Management Studio, it will both create the tables and load the data.

### Create Azure SQL Data Warehouse tables

Connect to the Data Warehouse using SQL Server Data Tools Preview in Visual Studio 2015 using the following settings:
   - Server name: **personal-[*UNIQUE*].database.windows.net**
   - Authentication: **SQL Server Authentication**
   - Login: **personaluser**
   - Password: **pass@word1**

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
               DateTimeStart DATETIME2,
               DateTimeStop DATETIME2,			   
	           EventId INT,
	           AverageRating FLOAT
            )
            WITH (
	           CLUSTERED COLUMNSTORE INDEX
            )

### Create the AML service

1. Browse: http://gallery.azureml.net/Details/aec6df682abf4a149bd7f2299cf2902f # copy this experiment from the gallery
1. Click: **Open in Studio**
1. Select: REGION: **[*REGION*]** # Up to you
1. Select: WORKSPACE: **[*WORKSPACE*]** # Your workspace
1. Click: **OK** > **Reader**
1. Type: Database server name: **personal-[*UNIQUE*].database.windows.net**
1. Type: Server user account password: **pass@word1**
1. Click: **Writer**
1. Type: Database server name: **personal-[*UNIQUE*].database.windows.net**
1. Type: Server user account password: **pass@word1**
1. Click: **RUN** > **DEPLOY WEB SERVICE** # See the debugging section for steps on how to test the service

### Edit and start the ASA job

1. Browse: https://manage.windowsazure.com
1. Click: **STREAM ANALYTICS** > **personalstreamanalytics[*unique*]** > **OUTPUTS** > **ADD OUTPUT**
1. Select: **Power BI**
1. Click: **Next** > **Authorize Now** # Login with your credentials
1. Type: OUTPUT ALIAS: **OutputPowerBI**
1. Type: DATASET NAME: **personalDB** # This dataset will be overwritten in PBI should it already exist
1. Type: TABLE NAME: **personalDB**
1. Select: WORKSPACE: **My Workspace** # Default
1. Click: **Finish** > **Start** > **Finish** # You do not need to specify a custom time

### Deploy the data generator as a Web Job

1. Download data generator: https://github.com/Azure/Cortana-Intelligence-Gallery-Content/blob/master/Tutorials/SQL-Data-Warehouse/datagenerator.zip
1. Unzip: **datagenerator.zip**
1. Edit: **Rage.exe.config**
1. Replace: EVENTHUBNAME: With: **personaleventhub[*UNIQUE*]**
1. Get CONNECTION STRING
    1. Browse: https://manage.windowsazure.com # Get the endpoint
    1. Click: SERVICE BUS
    1. Select: **personalservicebus[*UNIQUE*]**
    1. Click: CONNECTION INFORMATION
    1. Copy: CONNECTION STRING
1. Replace: ENDPOINT: With: CONNECTION STRING
1. Zip: **datagenerator.zip**
1. Browse: https://manage.windowsazure.com
1. Click: **NEW** > **COMPUTE** > **WEB APP** > **QUICK CREATE**
1. Type: **ratings[*UNIQUE*]**
1. Select: APP SERVICE PLAN: From your subscription
1. Click: **CREATE WEB APP** > **WEB APPS** > **ratings[*UNIQUE*]** > **WEBJOBS** > **ADD A JOB**
1. Type: NAME: **ratings[*UNIQUE*]**
1. Browse: **datagenerator.zip**
1. Select: HOW TO RUN: **Run continuously** # The default. It generates new ratings every 5 seconds.
1. Click: **Finish**

## Create the Data Factories

At this point you are ready to connect everything together. You will create two data factories. The first data factory will orchestrate data to be read from the SQL Data Warehouse by Azure Machine Learning, at which point the average rating is computed and written back to the SQL Data Warehouse. The second data factory will orchestrate copying data from the on prem SQL Server to the SQL Data Warehouse.

Click this button

<a href="https://portal.azure.com/#create/Microsoft.Template/uri/https%3A%2F%2Fraw.githubusercontent.com%2Froalexan%2FSolutionArchitects%2Fmaster%2Fazuredeploypart2.json" target="_blank">
    <img src="http://azuredeploy.net/deploybutton.png"/>
</a>

This will create a new "blade" in the Azure portal.

![arm1-image](./media/arm2.png)

1. Parameters
   1. Type: UNIQUE (string): **[*UNIQUE*]** # Use the one previously entered
   1. Select: LOCATION: **[*LOCATION*]** # Use the one previously selected
   1. URL
   1. API KEY
   1. Click: **OK**
1. Select: Subscription: **[*SUBSCRIPTION*]** # Use the one previously selected
1. Select: Resource group: **[*UNIQUE*]** # Use the one previously selected
1. Click: **Review legal terms** > **Create**
1. Check: **Pin to dashboard** # If you want it on your dashboard
1. Click: **Create**

## Create and Register the Data Management Gateway registration key

1. Create the ADF Data Management Gateway
    1. Browse: https://portal.azure.com
		1. Click: **Data factories** > **personal2ADF[*UNIQUE*]** > **Author and deploy** > **More commands** > **New data gateway**
		1. Type: Data gateway name: **datagateway-[*UNIQUE*]**
		1. Click: **OK**
		1. Copy: **NEW KEY**
1. Update the Data Management Gateway with the ADF registration key
    1. Start: Microsoft Data Management Gateway Configuration manager
		1. Click: **Change Key**
		1. Check: Show gateway key # To verify the key is correct
		1. Paste: **NEW KEY**
		1. Click: **OK**

## Start the on-prem pipelines

1. Browse: https://portal.azure.com
1. Click: **Data factories** > **personal2ADF[*UNIQUE*]** > **Author and deploy**
1. Expand: **Pipelines**
1. Select: **SQLDB-to-SQLDW-pipeline2**
1. Edit: **"isPaused": true** : to **"isPaused": false**
1. Click: **Deploy**

## Create the PBI dashboard

### Realtime visualization

![dashboard-usecase-realtime](./media/dashboard-usecase-realtime.png)

1. Browse: https://powerbi.microsoft.com
1. Click: **Sign in** # Login with your credentials
1. Show: The navigation pane
1. Click: **personalDB** # Under the Datasets folder > **Line chart** # Under Visualizations
1. Drag: **DateTime**: To: **Axis**
1. Drag: **DeviceId**: To: **Legend**
1. Drag: **Rating**: To: **Values**
1. Click: **Save**
1. Type: Name: **personalDB**
1. Click: **Save** > **Pin visual** # pin icon on upper-right
1. Select: **New dashboard**
1. Type: Name: **personalDB**
1. Click: **Pin**

### Predictive visualization

![dashboard-usecase-predictive](./media/dashboard-usecase-predictive.png)

1. Browse: https://powerbi.microsoft.com
1. Click: **Sign in** # Login with your credentials
1. Show: The navigation pane
1. Click: **Get Data** > Databases: **Get** > **Azure SQL Data Warehouse** > **Connect** > Server: **personal-[*UNIQUE*].database.windows.net** > Database: **personalDB** > **Next** > Username: **personaluser** > Password: **pass@word1** > **Sign in** > Datasets: **personalDB** > Visualizations: **Line Chart**
1. Expand: Fields: **AverageRatings**
1. Drag: **DateTimeStop**: To: **Axis**
1. Drag: **AverageRating**: To: **Values**
1. Drag: **EventId**: To: **Visual Level Filters**
1. Expand: **EventId**
1. Select: **Is**
1. Type: **1**
1. Click: **Apply filter**
1. Click: **Save**
1. Type: Name: **personalDB2**
1. Click: **Save** > Reports: **personalDB2** > **Pin icon**
1. Select: **Existing dashboard** > **personalDB**
1. Click: **Pin**
1. Select: Dashboards: **personalDB**
1. Resize tiles

### Using historical data

#### Realtime visualization

1. Edit: **personalDB2**
1. Click: **New page**
1. Click: **Line Chart**
1. Expand: **Ratings**
1. Drag: **DateTime**: To: **Axis**
1. Drag: **DeviceId**: To: **Legend**
1. Drag: **Rating**: To: **Values**
1. Drag: **EventId**: To: **Visual Level Filters**
1. Expand: **EventId**
1. Select: **Is**
1. Type: **2**
1. Click: **Apply filter** > **Save**

#### Predictive visualization

1. Click: **Line Chart**
1. Expand: Fields: **AverageRatings**
1. Drag: **DateTimeStop**: To: **Axis**
1. Drag: **AverageRating**: To: **Values**
1. Drag: **EventId**: To: **Visual Level Filters**
1. Expand: **EventId**
1. Select: **Is**
1. Type: **2**
1. Click: **Apply filter** > **Save**

#### Update dashboard

1. Click: **Save**
1. Select: **Existing dashboard** > **personalDB**
1. Click: **Pin**
1. Select: Dashboards: **personalDB**
1. Resize tiles

## Summary

Congratulations! If you made it to this point, you should have a running sample with real time and predictive pipelines showcasing the power of Azure SQL Data Warehouse and its integration with many of the other Azure services. The next section lists the steps to tear things down when you are done.

## Undeploy
1. Browse: https://portal.azure.com
1. Expand: Resource groups
1. Select: your resource group
1. Click: ...
1. Select: Delete

## Debugging

### Verify AML web service is working

1. Browse: https://studio.azureml.net
1. Click: **my experiments** > **WEB SERVICES**
1. Select: **Ratings** # Your web service
1. Click: **TEST** # Verify that request/response works
1. Click: **DATABASE QUERY**:
      SELECT
      CAST(Rating AS INT) AS Rating
      FROM Ratings
      WHERE EventId = 1
1. Click: DATABASE SERVER NAME: **personal-[*UNIQUE*].database.windows.net** > DATABASE NAME: **personalDB** > SERVER USER ACCOUNT NAME: **personaluser** > SERVER USER ACCOUNT PASSWORD: **pass@word1** > **OK**

### Verify data generator is working

#### From Portal

1. Browse: https://manage.windowsazure.com
1. Click: **personalstreamanalytics[*UNIQUE*]** > **DASHBOARD** > **Operation Logs**
1. Select: a recent log
1. Click: DETAILS

#### From SQL Client

1. Connect to the Data Warehouse using a SQL client of your choice
1. Run SQL to view the latest entries. For example:
	      select * from Ratings order by DateTime desc;
