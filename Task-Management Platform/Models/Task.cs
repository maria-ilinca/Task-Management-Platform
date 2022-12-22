using System.ComponentModel.DataAnnotations;

namespace Task_Management_Platform.Models
{
    public class Task
    {
        [Key]
        public int TaskId { get; set; }
        [Required(ErrorMessage = "Trebuie sa introduceti un nume pentru task")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Introduceti descrierea unui task")]
        public string Description { get; set; }
        [Required(ErrorMessage= "Fiecare task are nevoie de un status")]
        public string Status { get; set; }
        [Required(ErrorMessage = "Introduceti data de start a unui task")]
        public DateTime DataStart { get; set; }
        [Required(ErrorMessage = "Introduceti data de finalizare a taskului")]
        public DateTime DataFinalizare { get; set; }

        public string? OrganizerId { get; set; }

        public virtual ApplicationUser? Organizer { get; set; }

        public int? ProjectId { get; set; }

        public virtual Project? Project { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<TaskUser>? TaskUsers { get; set; }


    }
}
