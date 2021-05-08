using System.Threading.Tasks;
using CPTech.EFCore.Repositoies;
using Microsoft.EntityFrameworkCore;

namespace CPTech.EFCore.Repositories
{
    public class BaseRepository : IRepository
    {
        private readonly DbContext dbContext;

        public BaseRepository(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class
            => dbContext.FindAsync<TEntity>(keyValues);

        public Task<int> SaveChangesAsync() => dbContext.SaveChangesAsync();

        public Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Add(entity);
            return SaveChangesAsync();
        }

        public Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class
        {
            dbContext.Update(entity);
            return SaveChangesAsync();
        }
    }
}
