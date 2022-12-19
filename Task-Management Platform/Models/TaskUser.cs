using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management_Platform.Models
{
    public class TaskUser
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TaskId { get; set; }
        public int? UserId { get; set; }

        public virtual Task? Task { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
