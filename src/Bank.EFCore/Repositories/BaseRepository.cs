using System.Threading.Tasks;
using Bank.Domains.Repositoies;
using Microsoft.EntityFrameworkCore;

namespace Bank.EFCore.Repositories
{
    public class BaseRepository : IRepository
    {
        private readonly DbContext dbContext;

        public BaseRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
        {
            return dbContext.FindAsync<TEntity>(keyValues);
        }

        public Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Add(entity);
            return dbContext.SaveChangesAsync();
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Update(entity);
            return dbContext.SaveChangesAsync();
        }
    }
}
