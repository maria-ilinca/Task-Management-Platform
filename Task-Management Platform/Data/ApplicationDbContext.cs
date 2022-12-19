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


            modelBuilder.Entity<Comment>()
                .HasOne(c => c.Task)
                .WithMany(t => t.Comments)
                .HasForeignKey(c => c.TaskId);


        }

    }

    
}