using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Model.New {
    public class Account {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        [MaxLength(255)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(255)]
        public string LastName { get; set; }

        [Required]
        [MaxLength(255)]
        public string Email { get; set; }

        [Required]
        [MaxLength(64)]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime LastAccess { get; set; }

    }
}
