using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace WaterMeters {
    class TableRecord : TableEntity {
        public string MeterId { get; set; }
        public string DateTime { get; set; }
        public string Readings { get; set; }

        public TableRecord(string MeterId, string DateTime, string Readings) {
            this.PartitionKey = MeterId;
            this.RowKey = System.Guid.NewGuid().ToString();
            this.MeterId = MeterId;
            this.DateTime = DateTime;
            this.Readings = Readings;
        }
    }
}
