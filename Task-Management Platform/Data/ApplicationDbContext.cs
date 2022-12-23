using Task_Management_Platform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
using Task = Task_Management_Platform.Models.Task;
//using Microsoft.AspNetCore.Identity;

namespace Task_Management_Platform.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ApplicationUser> ApplicationUsers { get; set; }
        
        public DbSet<UserTeam> UserTeams { get; set; }

        public DbSet<UserTask> UserTasks { get; set; }
        public DbSet<Task> Tasks { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Project> Projects { get; set; }

        //protected override void OnModelCreating(ModelBuilder modelBuilder)
        //{

        //    base.OnModelCreating(modelBuilder);

        //    /* definire primary key compus
        //    modelBuilder.Entity<ArticleBookmark>()
        //        .HasKey(ab => new { ab.Id, ab.ArticleId, ab.BookmarkId });


        //    // definire relatii cu modelele Bookmark si Article (FK)
        //    modelBuilder.Entity<ArticleBookmark>()
        //        .HasOne(ab => ab.Article)
        //        .WithMany(ab => ab.ArticleBookmarks)
        //        .HasForeignKey(ab => ab.ArticleId);

        //    modelBuilder.Entity<ArticleBookmark>()
        //        .HasOne(ab => ab.Bookmark)
        //        .WithMany(ab => ab.ArticleBookmarks)
        //        .HasForeignKey(ab => ab.BookmarkId); */
        //} 

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ApplicationUser>()
                .Property(x => x.FirstName)
                .HasMaxLength(250);
            modelBuilder.Entity<ApplicationUser>()
                .Property(x => x.LastName)
                .HasMaxLength(250);

            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Task>()
                .HasOne(c => c.Project)
                .WithMany(c => c.Tasks)
                .HasForeignKey(c => c.ProjectId)
                .OnDelete(DeleteBehavior.ClientCascade);

            modelBuilder.Entity<Project>()
                .HasMany(p => p.Tasks)
                .WithOne(p => p.Project)
                .OnDelete(DeleteBehavior.ClientCascade);

            // TaskUser M-M
            //modelBuilder.Entity<TaskUser>()
            //    .HasOne(tu => tu.Task)
            //    .WithMany(tu => tu.TaskUsers)
            //    .HasForeignKey(tu => tu.TaskId)
            //    .OnDelete(DeleteBehavior.ClientCascade);
            


        }

    }

    
}