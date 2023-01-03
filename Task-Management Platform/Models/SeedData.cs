using Microsoft.EntityFrameworkCore;
using Task_Management_Platform.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration.UserSecrets;

namespace Task_Management_Platform.Models
{
    public static class SeedData
    {
        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService
                <DbContextOptions<ApplicationDbContext>>()))
            {
                // verificam daca exista vreun rol
                if (context.Roles.Any())
                {
                    return;
                }
                // daca nu exista niciunul, le cream noi
                context.Roles.AddRange(
                    new IdentityRole { Id = "2b81e1d6-fc64-4513-9509-9a07c21deea6", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                    new IdentityRole { Id = "2b81e1d6-fc64-4513-9509-9a07c21deea7", Name = "Organizer", NormalizedName = "Organizer".ToUpper() },
                    new IdentityRole { Id = "2b81e1d6-fc64-4513-9509-9a07c21deea8", Name = "User", NormalizedName = "User".ToUpper() }
                );

                // Pentru parole
                var hasher = new PasswordHasher<ApplicationUser>();

                // Adaugam si in BD userii cu roluri
                // cate unul pentru fiecare
                context.Users.AddRange(
                    new ApplicationUser
                    {
                        Id = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebb",
                        UserName = "admin@mail.com",
                        EmailConfirmed = true,
                        NormalizedEmail = "ADMIN@MAIL.COM",
                        Email = "admin@mail.com",
                        NormalizedUserName = "ADMIN@MAIL.COM",
                        PasswordHash = hasher.HashPassword(null, "Admin1!")

                    },

                    new ApplicationUser
                    {
                        Id = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebc",
                        UserName = "organizator@mail.com",
                        NormalizedEmail = "ORGANIZATOR@MAIL.COM",
                        Email = "organizator@mail.com",
                        NormalizedUserName = "ORGANIZATOR@MAIL.COM",
                        PasswordHash = hasher.HashPassword(null, "Organizator1!")
                    },

                    new ApplicationUser
                    {
                        Id = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebd",
                        UserName = "user@mail.com",
                        NormalizedEmail = "USER@MAIL.COM",
                        Email = "user@mail.com",
                        NormalizedUserName = "USER@MAIL.COM",
                        PasswordHash = hasher.HashPassword(null, "User1!")
                    }

                );

                // Asociem user cu roluri
                context.UserRoles.AddRange(
                    new IdentityUserRole<string>
                    {
                        RoleId = "2b81e1d6-fc64-4513-9509-9a07c21deea6",
                        UserId = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebb"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "2b81e1d6-fc64-4513-9509-9a07c21deea7",
                        UserId = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebc"
                    },
                    new IdentityUserRole<string>
                    {
                        RoleId = "2b81e1d6-fc64-4513-9509-9a07c21deea8",
                        UserId = "7ca94df4-6cd8-4a3e-a519-4277e5a77ebd"
                    }
                );

                context.SaveChanges();
            }
        }
    }
}
