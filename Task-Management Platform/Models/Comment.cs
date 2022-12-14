using System.ComponentModel.DataAnnotations;

namespace Task_Management_Platform.Models
{
        public class Comment
        {
            [Key]
            public int Id { get; set; }

            [Required(ErrorMessage = "Continutul comentariului este obligatoriu")]
            public string Content { get; set; }

            public DateTime Date { get; set; }

            public int? TaskId { get; set; }

            public string? UserId { get; set; }

            public virtual ApplicationUser? User { get; set; }

            public virtual Task? Task { get; set; }
        }

}
