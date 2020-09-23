using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;

namespace RemoteGitDeploy.Models.New {
    public class Team {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        public long CreatorId { get; set; }
        public Account Creator { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [MaxLength(10000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        public Team(long creatorId, string name, string description) {
            Id = Security.IdGen.GetId();
            Guid = System.Guid.NewGuid().ToString();
            CreatorId = creatorId;
            Name = name;
            Description = description;
            CreationDate = DateTime.UtcNow;
        }

    }
}
