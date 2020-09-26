using System.Globalization;
using RemoteGitDeploy.Models.New;

namespace RemoteGitDeploy.Models.Views {
    public class SnippetFileView {

        public long Id { get; set; }

        public string Filename { get; set; }

        public string Content { get; set; }

        public string Language { get; set; }

        public string CreationDate { get; set; }

        public SnippetFileView(SnippetFile snippetFile) {
            Id = snippetFile.Id;
            Filename = snippetFile.Filename;
            Content = snippetFile.Content;
            Language = snippetFile.Language;
            CreationDate = snippetFile.CreationDate.ToString(CultureInfo.InvariantCulture); ;
        }

    }
}
