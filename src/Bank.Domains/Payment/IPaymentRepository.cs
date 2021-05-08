using System.Threading.Tasks;
using Bank.Domains.Payment.Entities;
using CPTech.EFCore.Repositoies;

namespace Bank.Domains.Payment
{
    public interface IPaymentRepository : IRepository
    {
        Task<int> OrderCreateAsync(PayOrder order);
        Task<int> OrderCloseAsync(PayOrder order);
        Task<int> OrderCancelAsync(PayOrder order);
        Task<PayOrder> OrderQueryAsync(long orderNo);
        Task<PayNotify> SelectNotifyAsync(string tag);
        Task<int> AddPayOrderLogAsync(PayOrderLog payOrderLog);
        Task<int> UpdatePayOrderLogAsync(PayOrderLog payOrderLog);
    }
}
