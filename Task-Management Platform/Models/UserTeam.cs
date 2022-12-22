using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management_Platform.Models
{
    public class UserTeam
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public int? TeamId { get; set; }

        public virtual Team? Team { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
