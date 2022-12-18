using System.ComponentModel.DataAnnotations;

namespace Task_Management_Platform.Models
{
    public class Team
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Trebuie sa introduceti un nume pentru echipa voastra")]
        public string TeamName { get; set; }

        public virtual ICollection<Task>? Tasks { get; set; }
    }

}
