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
            // Create a new topology
            TopologyBuilder topologyBuilder = new TopologyBuilder("WaterMeters");

            // EventHubSpout

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

            // Decoding Bolt

            /*
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
                shuffleGrouping("EventHubSpout
                */

            topologyBuilder.SetBolt(
                "BoltDecoder",
                BoltDecoder.Get,
                new Dictionary<string, List<string>>()
                {
                    {Constants.DEFAULT_STREAM_ID, new List<string>(){"event"}}
                },
                partitionCount,
                true).
                DeclareCustomizedJavaSerializer(javaSerializerInfo).
                shuffleGrouping("EventHubSpout");

            // Azure Table Store Bolt

            /*
            topologyBuilder.SetBolt(
                "BoltAzureTableStorage",
                BoltAzureTableStorage.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                },
                1).fieldsGrouping("BoltDecoder", new List<int>() { 0 });
                */

            topologyBuilder.SetBolt(
                "BoltAzureTableStorage",
                BoltAzureTableStorage.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"event"}}
                },
                1).shuffleGrouping("BoltDecoder");

            // Azure SQL Bolt

            /*
            topologyBuilder.SetBolt(
                "BoltAzureSQL",
                BoltAzureSQL.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"word", "count"}}
                },
                1).fieldsGrouping("BoltDecoder", new List<int>() { 0 });
                */

            topologyBuilder.SetBolt(
                "BoltAzureSQL",
                BoltAzureSQL.Get,
                new Dictionary<string, List<string>>()
                {
            {Constants.DEFAULT_STREAM_ID, new List<string>(){"event"}}
                },
                1).shuffleGrouping("BoltDecoder");

            return topologyBuilder;
        }
    }
}

