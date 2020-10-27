using Bank.Domains.Payment;
using Bank.EFCore.Models;
using System.Threading.Tasks;

namespace Bank.Domains.Payment
{
    public interface IPaymentRepository
    {
        string OrderCreate(OrderPayDto order);

        Task<string> GetUserAsync();
    }
}
