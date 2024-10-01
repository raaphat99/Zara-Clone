using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.EFCore.Data
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Product>()
            .Property(product => product.ID)
            .UseIdentityColumn(1001, 1);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Category>()
            .HasOne(c => c.ParentCategory)
            .WithMany(c => c.Subcategories)
            .HasForeignKey(c => c.ParentCategoryID);
        }

        public DbSet<Product> Products { get; set; }
    }
}
