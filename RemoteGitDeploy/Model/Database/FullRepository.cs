using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class FullRepository {
        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("team")]
        public Team Team;

        public FullRepository(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Guid = reader.GetString(1);
            Name = reader.GetString(2);
            Description = reader.GetString(3);
            Team = new Team(
                reader.GetInt64(4),
                reader.GetString(5),
                reader.GetString(6)
            );
        }
    }
}