using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace Task_Management_Platform.Models
{
    public class ApplicationUser : IdentityUser
    {
        public virtual ICollection<Comment>? Comments { get; set; }

        public virtual ICollection<Task>? Tasks { get; set; }

        public virtual ICollection<Team>? Teams { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

        public ICollection<TaskUser>? TaskUsers { get; set; }
        public ICollection<UserTeam>? UserTeams { get; set; }
    }
}
