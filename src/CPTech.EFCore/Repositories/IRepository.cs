using System.Threading.Tasks;

namespace CPTech.EFCore.Repositoies
{
    public interface IRepository
    {
        ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        Task<int> SaveChangesAsync();
        Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}
