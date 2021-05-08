using System;
using Bank.Domains.Payment;
using Bank.Domains.Payment.Services;

namespace Bank.Services
{
    public class CcbService : IPaymentService
    {
        public BasePayRsp OrderClose(OrderQueryReq orderCloseReq)
        {
            throw new NotImplementedException();
        }

        public string OrderNotify(OrderNotifyReq orderNotifyReq)
        {
            throw new NotImplementedException();
        }

        public BasePayRsp OrderPay(OrderPayReq orderPayReq)
        {
            throw new NotImplementedException();
        }

        public OrderQueryRsp OrderQuery(OrderQueryReq orderQueryReq)
        {
            throw new NotImplementedException();
        }

        public BasePayRsp OrderRefund(BasePayReq orderRefundReq)
        {
            throw new NotImplementedException();
        }
    }
}
