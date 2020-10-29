using Bank.Domains.Repositoies;
using System.Threading.Tasks;

namespace Bank.Domains.Payment
{
    public interface IPaymentRepository : IRepository
    {
        Task<int> OrderCreateAsync(PayOrder order);
        Task<int> OrderCloseAsync(PayOrder order);
        Task<int> OrderCancelAsync(PayOrder order);
        Task<PayOrder> OrderQueryLastAsync(long orderNo);
    }
}
