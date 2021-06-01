using System.Linq;
using TASI.Backend.Domain.Products.Entities;
using TASI.Backend.Domain.Suppliers.Entities;
using TASI.Backend.Domain.Users.Entities;

namespace TASI.Backend.Infrastructure.Database
{
    public class DbInitializer
    {
        public static void Initialize(TasiContext context)
        {
            context.Database.EnsureCreated();

            if (context.Users.Any())
            {
                return;
            }

            context.Users.AddRange(
                new User
                {
                    Role = UserRole.SuperAdmin,
                    FullName = "Fahmi Noor Fiqri",
                    Username = "fahmi",
                    Password = "$2y$12$laGszlpJBlNGNMOvhJ0.gO5Y2ssjwp1U.xeRRxxoGY.lfP8Gh.M9G", // admin
                    Address = "Kp Sawah Baru",
                    Latitude = -6.9021,
                    Longitude = 107.594
                },
                new User
                {
                    Role = UserRole.Supervisor,
                    FullName = "Ryan Agus Saputra",
                    Username = "ryan",
                    Password = "$2y$12$laGszlpJBlNGNMOvhJ0.gO5Y2ssjwp1U.xeRRxxoGY.lfP8Gh.M9G", // admin
                    Address = "Kp Sawah Baru",
                    Latitude = -6.9021,
                    Longitude = 107.594
                },
                new User
                {
                    Role = UserRole.Manager,
                    FullName = "Issrahmi Berrly",
                    Username = "berrly",
                    Password = "$2y$12$laGszlpJBlNGNMOvhJ0.gO5Y2ssjwp1U.xeRRxxoGY.lfP8Gh.M9G", // admin
                    Address = "Kp Sawah Baru",
                    Latitude = -6.9021,
                    Longitude = 107.594
                },
                new User
                {
                    Role = UserRole.Customer,
                    FullName = "Bima Gusti Pratama",
                    Username = "bima",
                    Password = "$2y$12$laGszlpJBlNGNMOvhJ0.gO5Y2ssjwp1U.xeRRxxoGY.lfP8Gh.M9G", // admin
                    Address = "Kp Sawah Baru",
                    Latitude = -6.9021,
                    Longitude = 107.594
                });

            context.Suppliers.AddRange(
                new Supplier
                {
                    Name = "PT Air Prima",
                    Address = "Kp Sawah Baru Barengkok",
                    Latitude = -6.9021,
                    Longitude = 107.594,
                    ShippingCost = 0,
                },
                new Supplier
                {
                    Name = "PT Mata Air Dunia",
                    Address = "Bukit Sakinah Barengkok, Leuwiliang, 16640",
                    Latitude = -6.6028126,
                    Longitude = 106.6409201,
                    ShippingCost = 918153,
                });

            context.Products.AddRange(
                new Product
                {
                    Barcode = "0011223344",
                    Name = "Air Prima Gelas",
                    Stock = 10,
                    Unit = QuantityUnit.Piece,
                    Price = 1000,
                    Weight = 0.04,
                    CanManufacture = true
                },
                new Product
                {
                    Barcode = "1122334455",
                    Name = "Gelas Plastik",
                    Stock = 10,
                    Unit = QuantityUnit.Piece,
                    Price = 1000,
                    Weight = 0.01,
                    CanManufacture = false
                },
                new Product
                {
                    Barcode = "1122334466",
                    Name = "Air Minum",
                    Stock = 10,
                    Unit = QuantityUnit.Gallon,
                    Price = 1000,
                    Weight = 0.1,
                    CanManufacture = false
                });

            context.SaveChanges();
        }
    }
}
