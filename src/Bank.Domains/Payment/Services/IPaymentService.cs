using Bank.Domains.Payment.Services;

namespace Bank.Domains.Payment
{
    public interface IPaymentService
    {
        BasePayRsp OrderPay(OrderPayReq orderPayReq);
        OrderQueryRsp OrderQuery(OrderQueryReq orderQueryReq);
        BasePayRsp OrderClose(OrderQueryReq orderCloseReq);
        string OrderNotify(OrderNotifyReq orderNotifyReq);
        BasePayRsp OrderRefund(BasePayReq orderRefundReq);
    }
}
