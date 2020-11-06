using System;
using Bank.Domains.Payment;
using Bank.Domains.Payment.Services;

namespace Bank.Services
{
    public class CcbService : IPaymentService
    {
        public string OrderClose(OrderQueryReq payQuery)
        {
            throw new NotImplementedException();
        }

        public string OrderNotify(OrderNotifyReq payNotify)
        {
            throw new NotImplementedException();
        }

        public BasePayRsp OrderPay(OrderPayReq payment)
        {
            throw new NotImplementedException();
        }

        public BasePayRsp OrderQuery(OrderQueryReq payQuery)
        {
            throw new NotImplementedException();
        }

        OrderQueryRsp IPaymentService.OrderQuery(OrderQueryReq payQuery)
        {
            throw new NotImplementedException();
        }
    }
}
