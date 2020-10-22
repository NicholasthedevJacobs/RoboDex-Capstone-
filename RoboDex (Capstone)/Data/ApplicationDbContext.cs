using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RoboDex__Capstone_.Models;

namespace RoboDex__Capstone_.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<IdentityRole>()
            .HasData(
            new IdentityRole
            {
                Name = "RoboDexer",
                NormalizedName = "ROBODEXER"
            }
            );

            builder.Entity<ItemTags>()
                .HasKey(bc => new { bc.ItemId, bc.TagsId });
            builder.Entity<ItemTags>()
                .HasOne(bc => bc.Item)
                .WithMany(b => b.ItemTags)
                .HasForeignKey(bc => bc.ItemId);
            builder.Entity<ItemTags>()
                .HasOne(bc => bc.Tag)
                .WithMany(c => c.ItemTags)
                .HasForeignKey(bc => bc.TagsId);
        }


       
        public DbSet<RoboDexer> RoboDexer { get; set; }
        public DbSet<Inbox> Inbox { get; set; }
        public DbSet<Inventory> Inventory { get; set; }
        public DbSet<Followers> Followers { get; set; }
        public DbSet<Items> Items { get; set; }
        public DbSet<LocationPlace> LocationPlace { get; set; }
        public DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public DbSet<Tags> Tags { get; set; }
        public DbSet<ItemTags> ItemTags { get; set; }
    }
}
