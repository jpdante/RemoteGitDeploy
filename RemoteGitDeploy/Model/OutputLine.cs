using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model {
    public class OutputLine {

        [JsonProperty("data")]
        public readonly string Data;

        [JsonProperty("timespan")]
        public readonly int TimeSpan;

        public OutputLine(string data, int timeSpan) {
            Data = data;
            TimeSpan = timeSpan;
        }
    }
}
