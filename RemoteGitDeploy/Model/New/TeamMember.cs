using System.ComponentModel.DataAnnotations;

namespace RemoteGitDeploy.Model.New {
    public class TeamMember {

        [Key]
        public long Id { get; set; }

        [Required]
        public long TeamId { get; set; }
        public Team Team { get; set; }

        [Required]
        public long AccountId { get; set; }
        public Account Account { get; set; }

    }
}