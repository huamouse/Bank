namespace Bank.Domains.Payment
{
    public interface IPaymentService
    {
        string OrderPay(Payment payment);
        string OrderQuery(PayQuery payQuery);
        string OrderClose(PayQuery payQuery);
        string Notify(PayNotify payNotify);
    }

    public class BasePay
    {
        // 支付类型
        public PayTypeEnum PayType { get; set; }
        // 支付通道
        public ChannelEnum Channel { get; set; }
    }

    public class Payment : BasePay
    {
        // 订单号
        public string OrderNo { get; set; }
        // 付款方
        public string Payer { get; set; }
        // 收款方
        public string Payee { get; set; }
        // 支付金额，单位：分
        public long Amount { get; set; }
        // 支付事由
        public string Note { get; set; }
        // 通知url
        public string NotifyUrl { get; set; }
    }

    public class PayQuery : BasePay
    {
        // 订单号
        public string OrderNo { get; set; }
        public string FlowNo { get; set; }
        public QueryTypeEnum QueryType { get; set; }
    }

    public class PayNotify : BasePay
    {
        public string MsgId { get; set; }
    }
}
