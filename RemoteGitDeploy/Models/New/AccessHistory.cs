using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Models.New {
    public class AccessHistory {

        [Key]
        public long Id { get; set; }

        [Required]
        public long AccountId { get; set; }
        public Account Account { get; set; }

        [Required]
        [MaxLength(39)]
        public string Ip { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime AccessDate { get; set; }

        public AccessHistory(long accountId, string ip) {
            Id = Security.IdGen.GetId();
            AccountId = accountId;
            Ip = ip;
            AccessDate = DateTime.UtcNow;
        }

    }
}
