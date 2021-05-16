using System.Linq;

namespace TASI.Backend.Infrastructure.Database
{
    public class DbInitializer
    {
        public static void Initialize(TasiContext context)
        {
            context.Database.EnsureCreated();

            if (context.Logins.Any())
            {
                return;
            }

            //context.SaveChanges();
        }
    }
}
