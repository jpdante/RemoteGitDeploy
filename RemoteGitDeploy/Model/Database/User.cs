using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class User {

        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonProperty("firstName")]
        public string FirstName;

        [JsonProperty("lastName")]
        public string LastName;

        [JsonProperty("email")]
        public string Email;

        [JsonProperty("username")]
        public string Username;

        public User(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            FirstName = reader.GetString(1);
            LastName = reader.GetString(2);
            Email = reader.GetString(3);
            Username = reader.GetString(4);
        }

    }
}
