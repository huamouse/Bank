using Bank.Domains.Payment;
using Microsoft.EntityFrameworkCore;

namespace Bank.EFCore
{
    public partial class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PayOrder> PayOrder { get; set; }
    }
}
