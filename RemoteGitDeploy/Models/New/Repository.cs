using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Models.New {
    public class Repository {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        public long OwnerId { get; set; }
        public Account Owner { get; set; }

        [MaxLength(64)]
        [Required]
        public string Username { get; set; }

        [MaxLength(40)]
        [Required]
        public string PersonalAccessToken { get; set; }

        [Required]
        [MaxLength(64)]
        public string Name { get; set; }

        [Required]
        [MaxLength(255)]
        public string Git { get; set; }

        [Required]
        [MaxLength(64)]
        public string Branch { get; set; }

        public long TeamId { get; set; }
        public Team Team { get; set; }

        [MaxLength(10000)]
        public string Description { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        public Repository(long ownerId, string username, string personalAccessToken, string name, string git, string branch, long teamId, string description) {
            Id = Security.IdGen.GetId();
            Guid = System.Guid.NewGuid().ToString();
            OwnerId = ownerId;
            Username = username;
            PersonalAccessToken = personalAccessToken;
            Name = name;
            Git = git;
            Branch = branch;
            TeamId = teamId;
            Description = description;
            CreationDate = DateTime.UtcNow;
        }
    }
}
