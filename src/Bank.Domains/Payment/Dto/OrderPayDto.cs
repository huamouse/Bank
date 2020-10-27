using System.ComponentModel;

namespace Bank.Domains.Payment
{
    public class OrderPayDto
    {
        // 订单Id
        public long OrderId { get; set; }

        [Description("支付类型")]
        public string PayType { get; set; }

        [Description("支付通道")]
        public string Channel { get; set; }

        // 付款方
        public string Payer { get; set; }
    }
}
