﻿using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PracticeWebProjects.Data.Models;
using System.Reflection.Emit;

namespace PracticeWebProjects.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Chef> Chefs { get; set; }

        public DbSet<Dish> Dishes { get; set; }

        public DbSet<DishChef> DishesChefs { get; set; }

        public DbSet<DishType> DishTypes { get; set; }

        public DbSet<Sale> Sales { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<DishChef>().HasKey(dc => new { dc.ChefId, dc.DishId });

            builder.Entity<Dish>()
            .Property(d => d.Cost)
            .HasColumnType("decimal(12,2)");

            builder.Entity<DishType>()
                .HasData(new DishType()
                {
                    Id = 1,
                    Name = "Starter Dish"
                },
                
                new DishType() 
                { 
                    Id = 2,
                    Name = "Main Dish"
                },

                new DishType()
                {
                    Id = 3,
                    Name = "Dessert"
                },

                new DishType()
                {
                    Id = 4,
                    Name = "Side Dish"
                },

                new DishType()
                {
                    Id = 5,
                    Name = "Beverage"
                }
                ); 

            

            base.OnModelCreating(builder);
        }
    }
}
