﻿
using Bulky.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Bulky.DataAccess.Data
{
	public class ApplicationDbContext : IdentityDbContext<IdentityUser>
	{
		public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
		{

		}

		public DbSet<Category> Categories { get; set; } // this line of code will create table in db when we hit add-migration command with fields specefied in category model

		public DbSet<Product> Products { get; set; }
		public DbSet<ApplicationUser> ApplicationUsers { get; set; }
		public DbSet<Company> Companies { get; set; }
		public DbSet<ShoppingCart> ShoppingCarts { get; set; }
		public DbSet<OrderHeader> OrderHeaders { get; set; }
		public DbSet<OrderDetail> OrderDetails { get; set; }
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{

			base.OnModelCreating(modelBuilder);

			modelBuilder.Entity<Category>().HasData(
				new Category { Id = 1, Name = "Action", DisplayOrder = 1, IsVerified = true },
				new Category { Id = 2, Name = "SciFi", DisplayOrder = 2, IsVerified = true },
				new Category { Id = 3, Name = "History", DisplayOrder = 3, IsVerified = true }
				);

			modelBuilder.Entity<Company>().HasData(
				new Company {
					Id = 1, 
					Name = "Soluiont Tech", 
					StreetAddress = "123 Tech Street, Hamburg", 
					City = "Tech City", 
					State = "IL",
					PhoneNumber = "+923347464567", 
					PostalCode = "46300", },

				new Company
				{
					Id = 2,
					Name = "Vivid Tech",
					StreetAddress = "99 Vid Street Tech Street, Hamburg",
					City = "Vid City",
					State = "VD",
					PhoneNumber = "+9234580935858",
					PostalCode = "33300",
				},
				new Company {
					Id = 3, 
					Name = "Reader Club", 
					StreetAddress = "999 Main Street, Hamburg", 
					City = "Lala Land", 
					State = "IL",
					PhoneNumber = "+923455533334", 
					PostalCode = "44300", }
				
				


				);

			modelBuilder.Entity<Product>().HasData(
				new Product
				{
					Id = 1,
					Title = "Fortune of Time",
					Author = "Billy Spark",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "SWD9999001",
					ListPrice = 99,
					Price = 90,
					Price50 = 85,
					Price100 = 80,
					CategoryId = 1,
					ImageUrl = "",
				},
				new Product
				{
					Id = 2,
					Title = "Dark Skies",
					Author = "Nancy Hoover",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "CAW777777701",
					ListPrice = 40,
					Price = 30,
					Price50 = 25,
					Price100 = 20,
					CategoryId = 1,
					ImageUrl = "",

				},
				new Product
				{
					Id = 3,
					Title = "Vanish in the Sunset",
					Author = "Julian Button",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "RITO5555501",
					ListPrice = 55,
					Price = 50,
					Price50 = 40,
					Price100 = 35,
					CategoryId = 2,
					ImageUrl = "",

				},
				new Product
				{
					Id = 4,
					Title = "Cotton Candy",
					Author = "Abby Muscles",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "WS3333333301",
					ListPrice = 70,
					Price = 65,
					Price50 = 60,
					Price100 = 55,
					CategoryId = 2,
					ImageUrl = "",

				},
				new Product
				{
					Id = 5,
					Title = "Rock in the Ocean",
					Author = "Ron Parker",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "SOTJ1111111101",
					ListPrice = 30,
					Price = 27,
					Price50 = 25,
					Price100 = 20,
					CategoryId = 3,
					ImageUrl = "",

				},
				new Product
				{
					Id = 6,
					Title = "Leaves and Wonders",
					Author = "Laura Phantom",
					Description = "Praesent vitae sodales libero. Praesent molestie orci augue, vitae euismod velit sollicitudin ac. Praesent vestibulum facilisis nibh ut ultricies.\r\n\r\nNunc malesuada viverra ipsum sit amet tincidunt. ",
					ISBN = "FOT000000001",
					ListPrice = 25,
					Price = 23,
					Price50 = 22,
					Price100 = 20,
					CategoryId = 3,
					ImageUrl = "",
				}
				);
		}
	}
}
