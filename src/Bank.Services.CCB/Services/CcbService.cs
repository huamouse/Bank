using Bank.Domains.Payment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Bank.Services
{
    public class CcbService : IPaymentService
    {
        public string Notify(PayNotify payNotify)
        {
            throw new NotImplementedException();
        }

        public string OrderClose(PayQuery payQuery)
        {
            throw new NotImplementedException();
        }

        public string OrderPay(Payment payment)
        {
            throw new NotImplementedException();
        }

        public string OrderQuery(PayQuery payQuery)
        {
            throw new NotImplementedException();
        }
    }
}
