using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tester {
    public abstract class DecodedFrame : Frame {
        public string Emetteur { get; set; }
        public string Passerelle { get; set; }
        public DateTime DateRef { get; set; }
        public string Payload { get; set; }
        public string Suffixe01 { get; set; }
        public string Suffixe02 { get; set; }
        public string TypeTrame { get; set; }

        public abstract SensorType SensorType { get; }
        public abstract FrameType FrameType { get; }
        public GatewayType GatewayType { get; set; }
        public GatewaySubtype GatewaySubtype { get; set; }
        public abstract DecodingDirection Direction { get; }

        public abstract void ApplyAdditionalRules();
    }
}
