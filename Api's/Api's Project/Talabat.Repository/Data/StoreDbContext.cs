using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Order;
using Talabat.Repository.Data.Cofigurations;

namespace Talabat.Repository.Data
{
    public class StoreDbContext : DbContext
    {
        public StoreDbContext(DbContextOptions<StoreDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.ApplyConfiguration(new ProductCategoryConfigurations());
            //modelBuilder.ApplyConfiguration(new ProductBrandConfigurations());
            //modelBuilder.ApplyConfiguration(new ProductConfigurations());
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(p => p.ProductBrandId)
                    .IsRequired(false); // Make ProductBrandId nullable

                entity.Property(p => p.ProductTypeId)
                    .IsRequired(false); // Make ProductTypeId nullable

                entity.HasOne(p => p.ProductBrand)
                    .WithMany()
                    .HasForeignKey(p => p.ProductBrandId)
                    .OnDelete(DeleteBehavior.SetNull); // Handle null value

                entity.HasOne(p => p.ProductType)
                    .WithMany()
                    .HasForeignKey(p => p.ProductTypeId)
                    .OnDelete(DeleteBehavior.SetNull); // Handle null value

                modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

                base.OnModelCreating(modelBuilder);

            });
        }



        public DbSet<Product> Products { get; set; }
        public DbSet<ProductBrand> Brands { get; set; }
        public DbSet<ProductType> Types { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<DeliveryMethod> DeliveryMethods { get; set; }
    }

}
