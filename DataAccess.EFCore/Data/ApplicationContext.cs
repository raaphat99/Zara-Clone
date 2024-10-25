using Domain.Enums;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Data
{
    public class ApplicationContext : IdentityDbContext<User>
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { }

        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<UserAddress> UserAddresses { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserMeasurement> UserMeasurements { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<Wishlist> Wishlists { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<ShippingMethod> ShippingMethods { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Size> Sizes { get; set; }
        public DbSet<SizeType> SizeTypes { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Filter> Filters { get; set; }
        public DbSet<TrackingNumber> TrackingNumbers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentCategoryId);

            builder.Entity<ProductImage>()
            .Property(p => p.ImageType)
            .HasConversion<string>();

            builder.Entity<Order>()
            .Property(order => order.Status)
            .HasConversion<string>();

            builder.Entity<Size>()
            .Property(productSize => productSize.Value)
            .HasConversion<string>();

            builder.Entity<ProductVariant>()
            .Property(p => p.ProductColor)
            .HasConversion<string>();

            builder.Entity<ProductVariant>()
            .Property(p => p.ProductMaterial)
            .HasConversion<string>();

            builder.Entity<ShippingMethod>()
            .Property(method => method.Type)
            .HasConversion<string>();

            builder.Entity<SizeType>()
            .Property(type => type.Type)
            .HasConversion<string>();

            builder.Entity<Size>()
                .Property(size => size.Value)
                .HasConversion<string>();

            builder.Entity<Payment>()
            .Property(payment => payment.PaymentStatus)
            .HasConversion<string>();

            builder.Entity<Order>()
            .HasIndex(o => o.TrackingNumber)
            .IsUnique();

            builder.Entity<Category>()
            .HasOne(c => c.SizeType)
            .WithMany(st => st.Category)
            .HasForeignKey(c => c.SizeTypeId);

            base.OnModelCreating(builder);
        }

    }
}

