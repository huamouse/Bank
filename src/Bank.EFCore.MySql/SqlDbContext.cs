using Bank.Domains.Payment.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bank.EFCore
{
    public partial class SqlDbContext : DbContext
    {
        public SqlDbContext(DbContextOptions<SqlDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<PayOrder> PayOrders { get; set; }

        public virtual DbSet<PayNotify> PayNotifies { get; set; }

        public virtual DbSet<PayOrderLog> PayOrderLogs { get; set; }
    }
}
