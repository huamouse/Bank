using Bank.Domains.Payment.Services;

namespace Bank.Domains.Payment
{
    public interface IPaymentService
    {
        BasePayRsp OrderPay(OrderPayReq payment);
        OrderQueryRsp OrderQuery(OrderQueryReq payQuery);
        string OrderClose(OrderQueryReq orderCloseReq);
        string OrderNotify(OrderNotifyReq payNotify);
    }
}
