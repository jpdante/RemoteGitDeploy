using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace RemoteGitDeploy.Model.New {
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

    }
}
