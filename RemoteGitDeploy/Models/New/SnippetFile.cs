using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Models.New {
    public class SnippetFile {

        [Key]
        public long Id { get; set; }

        [Required]
        public long SnippetId { get; set; }
        public Snippet Snippet { get; set; }

        [Required]
        [MaxLength(255)]
        public string Filename { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        [MaxLength(32)]
        public string Language { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        public SnippetFile(long snippetId, string filename, string content, string language) {
            Id = Security.IdGen.GetId();
            SnippetId = snippetId;
            Filename = filename;
            Content = content;
            Language = language;
            CreationDate = DateTime.UtcNow;
        }

    }
}
