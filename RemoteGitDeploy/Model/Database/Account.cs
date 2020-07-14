using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class Account {

        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonIgnore]
        public string Password;

        [JsonIgnore]
        public string Salt;

        [JsonProperty("username")]
        public string Username;

        public Account(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Password = reader.GetString(1);
            Salt = reader.GetString(2);
            Username = reader.GetString(3);
        }

    }
}
