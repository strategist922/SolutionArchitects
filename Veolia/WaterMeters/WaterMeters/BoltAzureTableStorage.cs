using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;

namespace WaterMeters {
    public class BoltAzureTableStorage : ISCPBolt {
        private Context ctx;
        public bool enableAck = false;

        private CloudTable table;

        public BoltAzureTableStorage(Context ctx) {
            Context.Logger.Info("BoltAzureTableStorage constructor called");

            //Set the context
            this.ctx = ctx;

            // Declare Input and Output schemas
            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            // A tuple containing a string field - the word
            inputSchema.Add("default", new List<Type>() { typeof(string) });

            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, null));

            if (Context.Config.pluginConf.ContainsKey(Constants.NONTRANSACTIONAL_ENABLE_ACK)) {
                enableAck = (bool)(Context.Config.pluginConf[Constants.NONTRANSACTIONAL_ENABLE_ACK]);
            }
            Context.Logger.Info("enableAck: {0}", enableAck);

            InitializeAzureTableStorage();
        }

        /// <summary>
        /// A delegate method to return the instance of this class
        /// </summary>
        /// <param name="context">SCP Context, automatically passed by SCP.Net</param>
        /// <param name="parms"></param>
        /// <returns>An instance of the current class</returns>
        public static BoltAzureTableStorage Get(Context context, Dictionary<string, Object> parms) {
            return new BoltAzureTableStorage(context);
        }

        public void InitializeAzureTableStorage() {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(Properties.Settings.Default.StorageConnection);
            CloudTableClient tableClient = storageAccount.CreateCloudTableClient();
            table = tableClient.GetTableReference(Properties.Settings.Default.TableName);
            table.CreateIfNotExists();
        }

        public void Execute(SCPTuple tuple) {
            Context.Logger.Info("Execute enter");

            string value = (string)tuple.GetValue(0);
            Context.Logger.Info("value: " + value);
            if (value != null) {
                string[] ss = value.Split(new string[] { "," }, StringSplitOptions.None);
                string MeterId = ss[0];
                string DateTime = ss[1];
                string Reading = ss[2];
                TableRecord record = new TableRecord(MeterId, DateTime, Reading);
                TableOperation insertOperation = TableOperation.Insert(record);
                table.Execute(insertOperation);
            }

            Context.Logger.Info("Execute exit");
        }
    }
}