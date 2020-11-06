using System.Threading.Tasks;

namespace Bank.Domains.Repositoies
{
    public interface IRepository
    {
        ValueTask<TEntity> FindAsync<TEntity>(params object[] keyValues) where TEntity : class;
        Task<int> AddAsync<TEntity>(TEntity entity) where TEntity : class;
        Task<int> UpdateAsync<TEntity>(TEntity entity) where TEntity : class;
    }
}
