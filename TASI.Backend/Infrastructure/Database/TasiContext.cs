using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TASI.Backend.Domain.Orders.Entities;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Infrastructure.Database
{
    public class TasiContext : DbContext
    {
        public TasiContext(DbContextOptions<TasiContext> options) : base(options)
        {
        }
        
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatusHistory> StatusHistory { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupGeneratedDates(modelBuilder.Entity<OrderDetail>());
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Product);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Order);
                //.WithMany(x => x.OrderDetails);

            SetupGeneratedDates(modelBuilder.Entity<Order>());
            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order);
            modelBuilder.Entity<Order>()
                .HasMany(x => x.StatusHistory)
                .WithOne(x => x.Order);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Supplier)
                .WithOne()
                .HasForeignKey<Order>(x => x.SupplierId);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.PicUser)
                .WithOne()
                .HasForeignKey<Order>(x => x.PicUserId);
            
            SetupGeneratedDates(modelBuilder.Entity<Supplier>());
            modelBuilder.Entity<Supplier>()
                .HasIndex(x => x.Name)
                .IsUnique();

            SetupGeneratedDates(modelBuilder.Entity<Product>());
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Name)
                .IsUnique();
        }

        private void SetupGeneratedDates<T>(EntityTypeBuilder<T> entity) where T : class, IDaoEntity
        {
            entity.Property(x => x.ModifiedDate)
                //.HasDefaultValueSql("GETDATE()")      // for SQL Server
                .HasDefaultValueSql("DATETIME('NOW')")  // for SQLite
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
