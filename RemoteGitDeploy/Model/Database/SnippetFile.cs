using System.Data;
using Newtonsoft.Json;

namespace RemoteGitDeploy.Model.Database {
    public class SnippetFile {
        [JsonIgnore]
        public long Id;

        [JsonProperty("id")]
        public string IdString;

        [JsonIgnore]
        public long Snippet;

        [JsonProperty("snippet")]
        public string SnippetString;

        [JsonProperty("filename")]
        public string Filename;

        [JsonProperty("code")]
        public string Code;

        public SnippetFile(IDataRecord reader) {
            Id = reader.GetInt64(0);
            IdString = Id.ToString();
            Snippet = reader.GetInt64(1);
            SnippetString = Snippet.ToString();
            Filename = reader.GetString(2);
            Code = reader.GetString(3);
        }

        public SnippetFile(long id, long snippet, string filename, string code) {
            Id = id;
            IdString = Id.ToString();
            Snippet = snippet;
            SnippetString = Snippet.ToString();
            Filename = filename;
            Code = code;
        }
    }
}