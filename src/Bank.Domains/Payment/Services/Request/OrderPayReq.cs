namespace Bank.Domains.Payment.Services
{
    public class OrderPayReq : BasePayReq
    {
        // 订单号
        public string OrderNo { get; set; }
        // 付款方
        public string Payer { get; set; }
        // 支付金额，单位：分
        public long Amount { get; set; }
        // 支付事由
        public string Note { get; set; }
        // 通知url
        public string NotifyUrl { get; set; }
    }
}
