using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RemoteGitDeploy.Models.New {
    public class ActionHistory {

        [Key]
        public long Id { get; set; }

        [Required]
        [MaxLength(36)]
        public string Guid { get; set; }

        [Required]
        public long RepositoryId { get; set; }
        public Repository Repository { get; set; }

        public int Icon { get; set; }

        public string Name { get; set; }

        public string Parameters { get; set; }

        public string Log { get; set; }

        [Required]
        public int Status { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime StartTime { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime FinishTime { get; set; }

        [Required]
        [Column(TypeName = "TIMESTAMP")]
        public DateTime CreationDate { get; set; }

        public ActionHistory(string guid, long repositoryId, int icon, string name, string parameters, string log, int status, DateTime startTime, DateTime finishTime) {
            Id = Security.IdGen.GetId();
            Guid = guid;
            RepositoryId = repositoryId;
            Icon = icon;
            Name = name;
            Parameters = parameters;
            Log = log;
            Status = status;
            StartTime = startTime;
            FinishTime = finishTime;
            CreationDate = DateTime.UtcNow;
        }
    }
}
