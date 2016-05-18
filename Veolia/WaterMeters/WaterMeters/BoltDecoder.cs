using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading;
using Microsoft.SCP;
using Microsoft.SCP.Rpc.Generated;

namespace WaterMeters {
    public class BoltDecoder : ISCPBolt {
        private Context ctx;

        // Constructor
        public BoltDecoder(Context ctx) {
            Context.Logger.Info("BoltDecoder constructor called");
            this.ctx = ctx;

            // Declare Input and Output schemas
            Dictionary<string, List<Type>> inputSchema = new Dictionary<string, List<Type>>();
            // Input contains a tuple with a string field (the sentence)
            inputSchema.Add("default", new List<Type>() { typeof(string) });
            Dictionary<string, List<Type>> outputSchema = new Dictionary<string, List<Type>>();
            // Outbound contains a tuple with a string field (the word)
            outputSchema.Add("default", new List<Type>() { typeof(string) });
            this.ctx.DeclareComponentSchema(new ComponentStreamSchema(inputSchema, outputSchema));
        }

        // Get a new instance of the bolt
        public static BoltDecoder Get(Context ctx, Dictionary<string, Object> parms) {
            return new BoltDecoder(ctx);
        }

        // Called when a new tuple is available
        public void Execute(SCPTuple tuple) {
            Context.Logger.Info("Execute enter");

            string tupleString = tuple.GetString(0);
            string value = Utils.getValue(tupleString);
            Context.Logger.Info("value:" + value);
            if (value != null) {
                string[] ss = value.Split(new string[] { "," }, StringSplitOptions.None);
                string MeterId = ss[0];
                string DateTime = ss[1];
                string Reading = Utils.decode(ss[2]);
                StringBuilder sbDecoded = new StringBuilder(MeterId);
                sbDecoded.Append(",");
                sbDecoded.Append(DateTime);
                sbDecoded.Append(",");
                sbDecoded.Append(Reading);
                string decoded = sbDecoded.ToString();
                Context.Logger.Info("decoded:" + decoded);
                //Emit decoded reading
                this.ctx.Emit(new Values(decoded));
            }

            Context.Logger.Info("Execute exit");
        }
    }
}