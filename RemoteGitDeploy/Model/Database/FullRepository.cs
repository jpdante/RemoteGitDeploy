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

        [JsonProperty("git")]
        public string Git;

        [JsonProperty("name")]
        public string Name;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("team")]
        public Team Team;

        [JsonProperty("lastCommit")]
        public string LastCommit;

        [JsonProperty("lastUpdate")]
        public string LastUpdate;

        public FullRepository(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Guid = reader.GetString(1);
            Name = reader.GetString(2);
            Git = reader.GetString(3);
            Description = reader.GetString(4);
            Team = new Team(
                reader.GetInt64(5),
                reader.GetString(6),
                reader.GetString(7)
            );
        }
    }
}