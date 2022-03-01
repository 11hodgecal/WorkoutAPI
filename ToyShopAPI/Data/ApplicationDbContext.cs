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
        }

        //seeds the admin account
        private void SeedAdmin(ModelBuilder builder)
        {
            PasswordHasher<UserModel> hasher = new PasswordHasher<UserModel>();
            UserModel user = new UserModel();
            user.Id = "27b9af34-a133-43e2-8dd2-aef04ddb2b8c";
            user.UserName = "admin@admin.com";
            user.NormalizedUserName = "admin@admin.com".ToUpper();
            user.NormalizedEmail = "admin@admin.com".ToUpper();
            user.FirstName = "Admin";
            user.LastName = "";
            user.Email = "admin@admin.com";
            user.Role = "admin";
            user.LockoutEnabled = false;
            user.ConcurrencyStamp = "7b483dfe-e56c-4d5b-97cd-b32652794d29";
            user.PasswordHash = hasher.HashPassword(user, "Password123*");

            builder.Entity<UserModel>().HasData(user);

        }

    }
}
