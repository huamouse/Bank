namespace Bank.Domains.Payment.Services
{
    public class BasePayReq
    {
        // 支付类型
        public PayTypeEnum PayType { get; set; }
        // 支付通道
        public ChannelEnum Channel { get; set; }
        // 收款方
        public string Payee { get; set; }
        public string PayeeName { get; set; }
    }
}
