using System.Collections.Generic;
using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class Snippet {
        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonProperty("guid")]
        public string Guid;

        [JsonProperty("account")]
        public long Account;

        [JsonProperty("description")]
        public string Description;

        [JsonProperty("files")]
        public List<SnippetFile> SnippetFiles;

        public Snippet(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Guid = reader.GetString(1);
            Account = reader.GetInt64(2);
            Description = reader.GetString(3);
        }
    }
}