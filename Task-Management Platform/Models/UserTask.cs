using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management_Platform.Models
{
    public class UserTask
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public int? TaskId { get; set; }
        public string? UserId { get; set; }

        public virtual Task? Task { get; set; }
        public virtual ApplicationUser? ApplicationUser { get; set; }
    }
}
