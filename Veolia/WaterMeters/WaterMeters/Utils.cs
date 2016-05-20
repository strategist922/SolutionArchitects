using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaterMeters {
    class Utils {

        private static String HEADERS = "MeterId,DateTime,Readings";

        // Returns the value portion of an event from an EventHub.
        // The header and newline need to be removed
        public static string getValue(string eventHubEvent) {
            eventHubEvent = eventHubEvent.Replace('"', ' ').Trim();
            int i = eventHubEvent.IndexOf(HEADERS);
            if (i != -1) {
                return eventHubEvent.Substring(HEADERS.Length + i + 4); // Add 4 for \r\n
            }
            return null;
        }

        // Do really simple "decoding" - in this case, upcase
        // the given string.
        public static string decode(string s) {
            return s.ToUpper();
        }
    }
}
