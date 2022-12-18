using System.ComponentModel.DataAnnotations;


namespace Task_Management_Platform.Models
{
    public class Project
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Trebuie sa introduceti un nume pentru proiect")]
        public string Name { get; set; }
        public string? Description { get; set; }
        public string? UserId { get; set; }   
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<Task>? Tasks { get; set; }

    }
}
