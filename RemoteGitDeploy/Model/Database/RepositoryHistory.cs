using System.Collections.Generic;
using System.Data;
using HtcSharp.Core.Utils;
using IdGen;
using Newtonsoft.Json;
using RemoteGitDeploy.Utils;

namespace RemoteGitDeploy.Model.Database {
    public class RepositoryHistory {

        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonIgnore]
        public long RepositoryId;

        [JsonProperty("repository")]
        public string RepositoryIdString;

        [JsonProperty("icon")]
        public int Icon;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("date")]
        public string Date;

        [JsonProperty("parameters")]
        public List<Parameter> Parameters;

        public RepositoryHistory(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            RepositoryId = reader.GetInt64(1);
            RepositoryIdString = RepositoryId.ToString();
            Icon = reader.GetInt32(2);
            Name = reader.GetString(3);
            Date = Security.IdGen.FromId(Id).DateTimeOffset.DateTime.ToString("dd/MM/yyyy HH:mm:ss");
            Parameters = JsonConvert.DeserializeObject<List<Parameter>>(reader.GetString(4));
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
