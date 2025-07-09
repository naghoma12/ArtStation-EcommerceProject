using ArtStation.Core.Entities;
using ArtStation.Core.Entities.AI;
using ArtStation.Core.Entities.Identity;
using ArtStation.Core.Entities.Payment;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ArtStation.Repository.Data
{
    public class ArtStationDbContext : IdentityDbContext<AppUser,AppRole,int>
    {
        private readonly IConfiguration configuration;

        public ArtStationDbContext(DbContextOptions<ArtStationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
      
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            modelBuilder.Entity<Product>()
            .HasOne(p => p.User)
            .WithMany() // Or .WithMany(u => u.Products) if a reverse nav exists
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<PaymentTransaction>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.Property(x => x.Currency).HasMaxLength(10);
                entity.Property(x => x.Status).HasMaxLength(50);
            });

        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Sale> Sales { get; set; }
        public DbSet<ProductPhotos> ProductPhotos { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<ProductColor> ProductColors { get; set; }
        public DbSet<ProductFlavour> ProductFlavours { get; set; }
        public DbSet<Favourite> Favorites { get; set; }
        public DbSet<ProductSize> ProductSizes { get; set; }
        //public DbSet<Address> Address { get; set; }
        //public DbSet<Order> Orders { get; set; }
        //public DbSet<OrderItem> OrderItems { get; set; }
        //public DbSet<ShippingCost> ShippingCosts { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<NotificationRequest> Notifications { get; set; }
        public DbSet<Shipping> Shippings { get; set; }
        public DbSet<Banner> Banners { get; set; }
        public DbSet<ReviewLikes> ReviewLikes { get; set; }
        public DbSet<ProductForWhom> ProductForWhoms { get; set; }
        public DbSet<PaymentTransaction> PaymentTransactions { get; set; }
        public DbSet<ChatResponseRecommendation> Recommendations { get; set; }
        public DbSet<RecommendedProduct> RecommendedProducts { get; set; }
        public DbSet<SkinAnalysis> SkinAnalyses { get; set; }
        public DbSet<SkinMetric> SkinMetrics { get; set; }

    }
}
