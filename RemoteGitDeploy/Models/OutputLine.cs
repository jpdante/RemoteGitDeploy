using Newtonsoft.Json;

namespace RemoteGitDeploy.Models {
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
