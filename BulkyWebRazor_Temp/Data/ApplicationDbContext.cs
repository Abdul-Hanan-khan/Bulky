using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<Category> Categories { get; set; } // this line of code will create table in db when we hit add-migration command with fields specefied in category model
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData(
                new Category { Id = 1, Name = "Action", DisplayOrder = 1, IsVerified = true },
                new Category { Id = 2, Name = "SciFi", DisplayOrder = 2, IsVerified = true },
                new Category { Id = 3, Name = "History", DisplayOrder = 3, IsVerified = true }
                );
        }
    }
}
