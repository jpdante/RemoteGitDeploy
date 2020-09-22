using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace RemoteGitDeploy.Model.New {
    public class ActionHistory {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        public long RepositoryId { get; set; }
        public Repository Repository { get; set; }

        public string Name { get; set; }

        public string Parameters { get; set; }

        public string Log { get; set; }

        [Required]
        public int Status { get; set; }

    }
}
