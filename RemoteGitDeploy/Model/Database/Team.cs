using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class Team {
        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        public Team(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Name = reader.GetString(1);
            Description = reader.GetString(2);
        }

        public Team(long id, string name, string description) {
            Id = id;
            IdString = Id.ToString();
            Name = name;
            Description = description;
        }
    }
}