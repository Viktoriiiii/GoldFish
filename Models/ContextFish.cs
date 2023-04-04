using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;

namespace GoldFish.Models
{
    public partial class ContextFish : DbContext
    {
        public ContextFish()
            : base("name=ContextFish")
        {
        }

        public virtual DbSet<Category> Category { get; set; }
        public virtual DbSet<Client> Client { get; set; }
        public virtual DbSet<Company> Company { get; set; }
        public virtual DbSet<Hystory> Hystory { get; set; }
        public virtual DbSet<Manufacturer> Manufacturer { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderProduct> OrderProduct { get; set; }
        public virtual DbSet<Point> Point { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<Role> Role { get; set; }
        public virtual DbSet<Status> Status { get; set; }
        public virtual DbSet<Unit> Unit { get; set; }
        public virtual DbSet<User> User { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Client>()
                .HasMany(e => e.Order)
                .WithOptional(e => e.Client)
                .HasForeignKey(e => e.ClientID);

            modelBuilder.Entity<Product>()
                .Property(e => e.ProductCost)
                .HasPrecision(19, 2);

            modelBuilder.Entity<User>()
                .HasOptional(e => e.Client)
                .WithRequired(e => e.User);
        }
    }
}
