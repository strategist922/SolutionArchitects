1. Open Azure PowerShell and type in the following command
	a. sign in your account: Login-AzureRmAccount
	b. get the subscription: Get-AzureSubscription
	c. select the right subscription: Select-AzureSubscription
	d. create a resource group if you don't want to use these already existed in the subscription
		--replace the name <yourResourceGroupName> with the name you want to use
		New-AzureRmResourceGroup -Name <yourResourceGroupName> -Location "West US"
	e. run the first part of the ARM template
		-- <yourArmTestName> whatever a name
		-- <yourResourceGroupName> from step d
		New-AzureRmResourceGroupDeployment -Name <yourArmTestName> -ResourceGroupName <yourResourceGroupName> -TemplateFile <path\name of your arm template>  -unique <yourUniqueIdentifier> 
2. After the first part of ARM template is deployed, two blob containers need to create in the storage account
	a. personalstream
	b. scriptcontainer
3. Upload the following to the container "scriptcontainer"
	a. all files in /script/hive in the zip file
4. Run the following scripts in the database created. the sql scripts are under /script/SQL/
5. Run the second part of the ARM template
6. Start the ASAs
7. Start the data generator