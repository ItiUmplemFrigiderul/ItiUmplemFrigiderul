using ItiUmplemFrigiderul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ItiUmplemFrigiderul.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }


        public DbSet<Product> Products { get; set; }
        public DbSet<Farm> Farms { get; set; }
        public DbSet<FarmProduct> FarmProducts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ApplicationUser> Users { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<Cart> Carts { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ProductOrder> ProductOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder
        modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<FarmProduct>()
            .HasOne(ac => ac.Farm)
            .WithMany(ac => ac.FarmProducts)
            .HasForeignKey(ac => ac.FarmId);

            modelBuilder.Entity<FarmProduct>()
            .HasOne(ac => ac.Product)
            .WithMany(ac => ac.FarmProducts)
            .HasForeignKey(ac => ac.ProductId);

            modelBuilder.Entity<Product>()
            .HasOne(ac => ac.Category)
            .WithMany(ac => ac.Products)
            .HasForeignKey(ac => ac.CategoryId);

            modelBuilder.Entity<Review>()
            .HasOne(ac => ac.User)
            .WithMany(ac => ac.Reviews)
            .HasForeignKey(ac => ac.UserId);

            modelBuilder.Entity<Review>()
            .HasOne(ac => ac.FarmProduct)
            .WithMany(ac => ac.Reviews)
            .HasForeignKey(ac => ac.FarmProductId);

            modelBuilder.Entity<Order>()
            .HasOne(ac => ac.User)
            .WithMany(ac => ac.Orders)
            .HasForeignKey(ac => ac.UserId);

            modelBuilder.Entity<ProductOrder>()
            .HasOne(ac => ac.Order)
            .WithMany(ac => ac.ProductOrders)
            .HasForeignKey(ac => ac.OrderId);

            modelBuilder.Entity<ProductOrder>()
            .HasOne(ac => ac.FarmProduct)
            .WithMany(ac => ac.ProductOrders)
            .HasForeignKey(ac => ac.FarmProductId);

            //modelBuilder.Entity<Farm>()
            //.HasOne(a => a.User)
            //.WithMany(c => c.Farms)
            //.HasForeignKey(a => a.UserId);

            modelBuilder.Entity<Cart>()
            .HasOne<ApplicationUser>(a => a.User)
            .WithOne(c => c.Cart)
            .HasForeignKey<Cart>(a => a.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Cart>()
            .HasOne(a => a.FarmProduct)
            .WithMany(c => c.Carts)
            .HasForeignKey(a => a.FarmProductId)
            .OnDelete(DeleteBehavior.Restrict);


        }


    }
};

