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

    }
}
