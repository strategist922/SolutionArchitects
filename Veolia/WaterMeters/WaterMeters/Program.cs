using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SCP;
using Microsoft.SCP.Topology;

namespace WaterMeters {
    [Active(true)]
    class Program : TopologyDescriptor {
        static void Main(string[] args) {
        }

        public ITopologyBuilder GetTopologyBuilder() {
            // Create a new topology named 'WaterMeters'
            TopologyBuilder topologyBuilder = new TopologyBuilder("WaterMeters");

            // Add the spout to the topology.
            // Name the component 'sentences'
            // Name the field that is emitted as 'sentence'
            /*
            topologyBuilder.SetSpout(
                "sentences",
                Spout.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"sentence"}}
                },
                1);
                */

            int partitionCount = Properties.Settings.Default.EventHubPartitionCount;
            EventHubSpoutConfig ehConfig = new EventHubSpoutConfig(
                    Properties.Settings.Default.EventHubPolicyName,
                    Properties.Settings.Default.EventHubPolicyKey,
                    Properties.Settings.Default.EventHubNamespace,
                    Properties.Settings.Default.EventHubName,
                    partitionCount);
            topologyBuilder.SetEventHubSpout(
                "EventHubSpout",
                ehConfig,
                partitionCount);

            List<string> javaSerializerInfo = new List<string>() { "microsoft.scp.storm.multilang.CustomizedInteropJSONSerializer" };

            // Add the splitter bolt to the topology.
            // Name the component 'splitter'
            // Name the field that is emitted 'word'
            // Use suffleGrouping to distribute incoming tuples
            //   from the 'sentences' spout across instances
            //   of the splitter
            /*
            topologyBuilder.SetBolt(
                "splitter",
                Splitter.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word"}}
                },
                1).shuffleGrouping("sentences");
                */
            topologyBuilder.SetBolt(
                "BoltDecoder",
                BoltDecoder.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word"}}
                },
                partitionCount,
                true).
                DeclareCustomizedJavaSerializer(javaSerializerInfo).
                shuffleGrouping("EventHubSpout");

            // Add the counter bolt to the topology.
            // Name the component 'counter'
            // Name the fields that are emitted 'word' and 'count'
            // Use fieldsGrouping to ensure that tuples are routed
            //   to counter instances based on the contents of field
            //   position 0 (the word). This could also have been
            //   List<string>(){"word"}.
            //   This ensures that the word 'jumped', for example, will always
            //   go to the same instance
            /*
            topologyBuilder.SetBolt(
                "counter",
                Counter.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                },
                1).fieldsGrouping("BoltDecoder", new List<int>() { 0 });
                */

            // Add the Azure SQL bolt to the topology.
            topologyBuilder.SetBolt(
                "BoltAzureTableStorage",
                BoltAzureTableStorage.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                },
                1).fieldsGrouping("BoltDecoder", new List<int>() { 0 });

            // Add the Azure SQL bolt to the topology.
            topologyBuilder.SetBolt(
                "BoltAzureSQL",
                BoltAzureSQL.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                },
                1).fieldsGrouping("BoltDecoder", new List<int>() { 0 });

            // Add topology config
            topologyBuilder.SetTopologyConfig(new Dictionary<string, string>()
            {
        {"topology.kryo.register","[\"[B\"]"}
    });

            return topologyBuilder;
        }
    }
}

