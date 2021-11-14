using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TASI.Backend.Domain.Manufacture.Entities;
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

        public DbSet<User> Users { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }
        public DbSet<OrderStatus> OrderStatus { get; set; }

        public DbSet<ManufactureJob> Manufacture { get; set; }
        public DbSet<ManufactureMaterial> ManufactureMaterial { get; set; }
        public DbSet<ManufactureStatus> ManufactureStatus { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            SetupGeneratedDates(modelBuilder.Entity<User>());
            modelBuilder.Entity<User>()
                .HasIndex(x => x.FullName)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(x => x.Username)
                .IsUnique();

            SetupGeneratedDates(modelBuilder.Entity<Product>());
            modelBuilder.Entity<Product>()
                .HasIndex(x => x.Name)
                .IsUnique();

            SetupGeneratedDates(modelBuilder.Entity<Supplier>());
            modelBuilder.Entity<Supplier>()
                .HasIndex(x => x.Name)
                .IsUnique();

            SetupGeneratedDates(modelBuilder.Entity<Order>());
            modelBuilder.Entity<Order>()
                .HasMany(x => x.StatusHistory)
                .WithOne(x => x.Order);
            modelBuilder.Entity<Order>()
                .HasMany(x => x.OrderDetails)
                .WithOne(x => x.Order);
            modelBuilder.Entity<Order>()
                .HasOne(x => x.Supplier)
                .WithMany();
            modelBuilder.Entity<Order>()
                .HasOne(x => x.PicUser)
                .WithMany();

            SetupGeneratedDates(modelBuilder.Entity<OrderStatus>());
            modelBuilder.Entity<OrderStatus>()
                .HasOne(x => x.Order);

            SetupGeneratedDates(modelBuilder.Entity<OrderDetail>());
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Product);
            modelBuilder.Entity<OrderDetail>()
                .HasOne(x => x.Order);

            SetupGeneratedDates(modelBuilder.Entity<ManufactureJob>());
            modelBuilder.Entity<ManufactureJob>()
                .HasMany(x => x.StatusHistory)
                .WithOne(x => x.Order);
            modelBuilder.Entity<ManufactureJob>()
                .HasMany(x => x.Materials)
                .WithOne(x => x.Order);
            modelBuilder.Entity<ManufactureJob>()
                .HasOne(x => x.Product)
                .WithMany();

            SetupGeneratedDates(modelBuilder.Entity<ManufactureStatus>());
            modelBuilder.Entity<ManufactureStatus>()
                .HasOne(x => x.Order);

            SetupGeneratedDates(modelBuilder.Entity<ManufactureMaterial>());
            modelBuilder.Entity<ManufactureMaterial>()
                .HasOne(x => x.Product);
            modelBuilder.Entity<ManufactureMaterial>()
                .HasOne(x => x.Order);
        }

        private void SetupGeneratedDates<T>(EntityTypeBuilder<T> entity) where T : class, IDaoEntity
        {
            var sql = Database.IsSqlite()
                ? "DATETIME('NOW')" // for SQLite
                : "GETDATE()"; // for SQL Server

            entity.Property(x => x.ModifiedDate)
                .HasDefaultValueSql(sql)
                .ValueGeneratedOnAddOrUpdate();
        }
    }
}
