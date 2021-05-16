using System.Linq;
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

            var user = new User
            {
                Role = UserRole.SuperAdmin,
                FullName = "Administrator K",
                Username = "admin",
                Password = "$2y$12$laGszlpJBlNGNMOvhJ0.gO5Y2ssjwp1U.xeRRxxoGY.lfP8Gh.M9G" // admin
            };

            context.Users.Add(user);

            context.SaveChanges();
        }
    }
}
