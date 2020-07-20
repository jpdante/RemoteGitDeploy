using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.Model.Database {
    public class ActionHistory {

        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonProperty("guid")]
        public string Guid;

        [JsonIgnore]
        public long RepositoryId;

        [JsonProperty("repository")]
        public string RepositoryIdString;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("date")]
        public string Date;

        [JsonProperty("parameters")]
        public List<Parameter> Parameters;

        [JsonProperty("logs")]
        public List<OutputLine> Logs;

        [JsonProperty("success")]
        public bool Success;

        public ActionHistory(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Guid = reader.GetString(1);
            RepositoryId = reader.GetInt64(2);
            RepositoryIdString = RepositoryId.ToString();
            Name = reader.GetString(3);
            Date = StaticData.IdGenerator.FromId(Id).DateTimeOffset.DateTime.ToString("dd/MM/yyyy HH:mm:ss");
            Parameters = JsonConvert.DeserializeObject<List<Parameter>>(reader.GetString(4));
            Logs = JsonConvert.DeserializeObject<List<OutputLine>>(reader.GetString(5));
            Success = reader.GetBoolean(6);
        }

        public class Parameter {

            [JsonProperty("name")]
            public string Name;

            [JsonProperty("value")]
            public string Value;

            [JsonProperty("color")]
            public string Color;

            public Parameter(string name, string value, string color) {
                Name = name;
                Value = value;
                Color = color;
            }

        }
    }
}
