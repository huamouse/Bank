using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CPTech.EFCore.Extensions
{
    public static class DatabaseExtension
    {
        public static void UseMigration(this DbContext dbContext)
        {
            if (dbContext.Database.GetPendingMigrations().Any())
                dbContext.Database.Migrate();
        }
    }
}
