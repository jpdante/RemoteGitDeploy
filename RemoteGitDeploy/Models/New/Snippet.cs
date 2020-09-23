using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Models.New {
    public class Snippet {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        public long CreatorId { get; set; }
        public Account Creator { get; set; }

        [MaxLength(100000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        public Snippet(long creatorId, string description) {
            Id = Security.IdGen.GetId();
            Guid = System.Guid.NewGuid().ToString();
            CreatorId = creatorId;
            Description = description;
            CreationDate = DateTime.UtcNow;
        }
    }
}
