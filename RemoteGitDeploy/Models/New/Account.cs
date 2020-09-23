using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using RemoteGitDeploy.Core;

namespace RemoteGitDeploy.Models.New {
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
        public Permission Permissions { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime LastAccess { get; set; }

        public Account(string firstName, string lastName, string email, string username, string password, Permission permissions) {
            Id = Security.IdGen.GetId();
            Guid = System.Guid.NewGuid().ToString();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Username = username;
            Password = password;
            Permissions = permissions;
            CreationDate = DateTime.UtcNow;
            LastAccess = DateTime.UtcNow;
        }
    }
}
