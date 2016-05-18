using Microsoft.Practices.EnterpriseLibrary.TransientFaultHandling;
using Microsoft.SCP;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WaterMeters {
    /// <summary>
    /// OVERVIEW:
    /// SqlAzureWriterBolt - A bolt that can insert, upsert or delete from Sql Azure.
    /// Change or override the execute method to as per your requirements.
    /// The bolt uses parameterized T-SQL statements based on specified table schema
    /// 
    /// PRE-REQUISITES:
    /// 1. Sql Azure Server, Database and Table - All values need to be specifed in AppSettings
    ///   a. SqlAzureConnectionString - your Sql Azure server connection string
    ///   b. SqlAzureTableName - your Sql Azure table name
    ///   c. SqlAzureTableColumns - comma separated column names, required for query building
    /// 2. Sql Table Schema  
    ///   a. Note that Sql Azure requires your table schema to have a clustered index
    ///   b. This can be easily acheived by including an identity column as the primary key which gets automatically generated on each insert
    ///   c. For e.g.: Create Table #your_table# ([ID] INT IDENTITY PRIMARY KEY, ...
    ///   
    /// ASSUMPTIONS:
    /// 1. The type infering of tuple field (SqlParameter) is left to the provider as we use AddWithValue. To avoid surprises you can specify SqlDbType directly.
    /// 
    /// NUGET: 
    /// 1. SCP.Net - http://www.nuget.org/packages/Microsoft.SCP.Net.SDK/
    /// 2. Sql Transient Fault handling - http://www.nuget.org/packages/EnterpriseLibrary.TransientFaultHandling.Data
    /// 
    /// REFERENCES:
    /// 1. Reliably connect to Azure SQL Database - https://msdn.microsoft.com/en-us/library/azure/dn864744.aspx
    /// </summary>
    public class BoltAzureSQL : ISCPBolt {
        public Context context;
        public bool enableAck = false;

        public String SqlConnectionString { get; set; }
        public ReliableSqlConnection SqlConnection { get; set; }

        RetryPolicy ConnectionRetryPolicy { get; set; }
        RetryPolicy CommandRetryPolicy { get; set; }

        public BoltAzureSQL(Context context) {
            Context.Logger.Info("BoltAzureSQL constructor called");

            //Set the context
            this.context = context;

            // Declare Input and Output schemas
            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            // A tuple containing a string field - the word
            inputSchema.Add("default", new List<Type>() { typeof(string) });

            this.context.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, null));

            if (Context.Config.pluginConf.ContainsKey(Constants.NONTRANSACTIONAL_ENABLE_ACK)) {
                enableAck = (bool)(Context.Config.pluginConf[Constants.NONTRANSACTIONAL_ENABLE_ACK]);
            }
            Context.Logger.Info("enableAck: {0}", enableAck);

            InitializeSqlAzure();
        }

        /// <summary>
        /// A delegate method to return the instance of this class
        /// </summary>
        /// <param name="context">SCP Context, automatically passed by SCP.Net</param>
        /// <param name="parms"></param>
        /// <returns>An instance of the current class</returns>
        public static BoltAzureSQL Get(Context context, Dictionary<string, Object> parms) {
            return new BoltAzureSQL(context);
        }

        /// <summary>
        /// Initialize the Sql Azure settings and connections
        /// </summary>
        public void InitializeSqlAzure() {
            this.SqlConnectionString = Properties.Settings.Default.SqlAzureConnectionString;
            if (String.IsNullOrWhiteSpace(this.SqlConnectionString)) {
                throw new ArgumentException("A required AppSetting cannot be null or empty", "SqlAzureConnectionString");
            }

            //Reference: https://msdn.microsoft.com/en-us/library/azure/dn864744.aspx
            //1. Define an Exponential Backoff retry strategy for Azure SQL Database throttling (ExponentialBackoff Class). An exponential back-off strategy will gracefully back off the load on the service.
            int retryCount = 4;
            int minBackoffDelayMilliseconds = 2000;
            int maxBackoffDelayMilliseconds = 8000;
            int deltaBackoffMilliseconds = 2000;

            ExponentialBackoff exponentialBackoffStrategy =
                new ExponentialBackoff("exponentialBackoffStrategy",
                    retryCount,
                    TimeSpan.FromMilliseconds(minBackoffDelayMilliseconds),
                    TimeSpan.FromMilliseconds(maxBackoffDelayMilliseconds),
                    TimeSpan.FromMilliseconds(deltaBackoffMilliseconds));

            //2. Set a default strategy to Exponential Backoff.
            RetryManager manager = new RetryManager(
                new List<RetryStrategy>
                {
                    exponentialBackoffStrategy
                },
                "exponentialBackoffStrategy");

            //3. Set a default Retry Manager. A RetryManager provides retry functionality, or if you are using declarative configuration, you can invoke the RetryPolicyFactory.CreateDefault
            RetryManager.SetDefault(manager);

            //4. Define a default SQL Connection retry policy and SQL Command retry policy. A policy provides a retry mechanism for unreliable actions and transient conditions.
            ConnectionRetryPolicy = manager.GetDefaultSqlConnectionRetryPolicy();
            CommandRetryPolicy = manager.GetDefaultSqlCommandRetryPolicy();

            //5. Create a function that will retry the connection using a ReliableSqlConnection.
            InitializeSqlAzureConnection();
        }

        /// <summary>
        /// Initialize Sql Azure Connection if not open
        /// </summary>
        public void InitializeSqlAzureConnection() {
            try {
                if (this.SqlConnection == null) {
                    this.SqlConnection = new ReliableSqlConnection(this.SqlConnectionString, ConnectionRetryPolicy, CommandRetryPolicy);
                }

                if (this.SqlConnection.State != ConnectionState.Open) {
                    ConnectionRetryPolicy.ExecuteAction(() => {
                        this.SqlConnection.Open();
                    });
                }
            }
            catch (Exception ex) {
                HandleException(ex);
                throw;
            }
        }

        public void ExecuteCommand(SqlCommand sqlCommand) {
            try {
                Context.Logger.Info("Executing Command: {0}", sqlCommand.CommandText);
                var rowsAffected = this.SqlConnection.ExecuteCommand(sqlCommand, CommandRetryPolicy);
                Context.Logger.Info("RowsAffected: {0}", rowsAffected);
            }
            catch (Exception ex) {
                Context.Logger.Error("Exception encountered while executing command. Command: {0}", sqlCommand.CommandText);
                HandleException(ex);
                throw;
            }
        }

        public void Execute(SCPTuple tuple) {
            try {
                Context.Logger.Info("Execute enter");

                string value = tuple.GetString(0);
                Context.Logger.Info("value:" + value);
                if (value != null) {
                    string[] ss = value.Split(new string[] { "," }, StringSplitOptions.None);
                    string MeterId = ss[0];
                    string DateTime = ss[1];
                    string Reading = ss[2];
                    String query = "INSERT INTO MeterReadings(DateTime,MeterId,Reading) VALUES(@DateTime,@MeterId,@Reading)";
                    SqlCommand command = new SqlCommand(query);
                    command.Parameters.AddWithValue("@DateTime", DateTime);
                    command.Parameters.AddWithValue("@MeterId", MeterId);
                    command.Parameters.AddWithValue("@Reading", Reading);
                    ExecuteCommand(command);
                }
            }
            catch (Exception ex) {
                Context.Logger.Error("An error occured while executing Tuple Id: {0}. Exception Details:\r\n{1}",
                    tuple.GetTupleId(), ex.ToString());

                //Fail the tuple if enableAck is set to true in TopologyBuilder so that the tuple is replayed.
                if (enableAck) {
                    this.context.Fail(tuple);
                }
            }
        }

        /// <summary>
        /// Log the exception and reset any prepared commands and connections
        /// </summary>
        /// <param name="ex">The exception thrown</param>
        public void HandleException(Exception ex) {
            StackFrame frame = new StackFrame(1);
            MethodBase method = frame.GetMethod();
            Context.Logger.Error("{0} threw an exception. Exception Details: {1}", method.Name, ex.ToString());
            Context.Logger.Info("Resetting all commands and connections");

            if (this.SqlConnection != null) {
                if (this.SqlConnection.State == ConnectionState.Open) {
                    this.SqlConnection.Close();
                }
                this.SqlConnection.Dispose();
                this.SqlConnection = null;
            }
        }
    }
}