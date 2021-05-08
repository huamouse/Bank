using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Required]
        public string Payer { get; set; }

        public string PayerName { get; set; }

        // 付款方
        [Required]
        public string Payee { get; set; }
        [RegularExpression(@"^(([0-9]|([1-9][0-9]{0,9}))((\.[0-9]{1,2})?))$")]
        public decimal Amount { get; set; }
    }
}
