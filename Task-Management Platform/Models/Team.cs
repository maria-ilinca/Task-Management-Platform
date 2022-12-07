using System.ComponentModel.DataAnnotations;

namespace ArticlesApp.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Numele categoriei este obligatoriu")]
        public string TeamName { get; set; }

        public virtual ICollection<Task> Tasks { get; set; }
    }

}
