using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WorkoutAPI.Models;

namespace WorkoutAPI.Data
{
    public class ApplicationDbContext : IdentityDbContext<UserModel>
    {
        public DbSet<ActivityModel> Activities { get; set; }
        public DbSet<WorkoutModel> Workouts { get; set; }
        public DbSet<UserModel> Users { get; set; }
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {


            base.OnModelCreating(builder);
            SeedAdmin(builder);
            SeedRoles(builder);
            SeedUserRoles(builder);
        }
        private void SeedRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityRole>().HasData(
                new IdentityRole()
                {
                    Id = "78bf8cbe-1f70-4d6d-890b-247bc57e6150",
                    Name = "Admin",
                    NormalizedName = "Admin".ToUpper(),
                    ConcurrencyStamp = "7b483dfe-e56c-4d5b-97cd-b32652794d29"

                }
                );

            builder.Entity<IdentityRole>().HasData(
               new IdentityRole()
               {
                   Id = "ecfbe7ad-bb6b-49e6-ac2b-6359a73fbf02",
                   Name = "user",
                   NormalizedName = "user".ToUpper(),
                   ConcurrencyStamp = "d4e41d27-8605-4e69-8587-2636ed98e286"

               }
               );
        }


        private void SeedAdmin(ModelBuilder builder)
        {
            PasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();
            UserModel user = new UserModel();
            user.Id = "27b9af34-a133-43e2-8dd2-aef04ddb2b8c";
            user.UserName = "admin@admin.com";
            user.NormalizedUserName = "admin@admin.com".ToUpper();
            user.NormalizedEmail = "admin@admin.com".ToUpper();
            user.Email = "admin@admin.com";
            user.Role = "admin";
            user.LockoutEnabled = false;
            user.ConcurrencyStamp = "7b483dfe-e56c-4d5b-97cd-b32652794d29";
            user.PasswordHash = hasher.HashPassword(user, "Admin123!");

            builder.Entity<UserModel>().HasData(user);

        }

        private void SeedUserRoles(ModelBuilder builder)
        {
            builder.Entity<IdentityUserRole<string>>().HasData(

                new IdentityUserRole<string>()
                {
                    RoleId = "78bf8cbe-1f70-4d6d-890b-247bc57e6150",
                    UserId = "27b9af34-a133-43e2-8dd2-aef04ddb2b8c"
                });

           

        }
    }
}
