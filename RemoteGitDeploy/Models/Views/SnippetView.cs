using System.Globalization;
using RemoteGitDeploy.Core;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class SnippetView {

        public long Id { get; set; }

        public string Guid { get; set; }

        public string Description { get; set; }

        public SnippetFileView[] Files { get; set; }

        public string CreationDate { get; set; }

        public SnippetView(Snippet snippet, SnippetFileView[] snippetFiles) {
            Id = snippet.Id;
            Guid = snippet.Guid;
            Description = snippet.Description;
            Files = snippetFiles;
            CreationDate = snippet.CreationDate.ToString(CultureInfo.InvariantCulture); ;
        }

    }
}
