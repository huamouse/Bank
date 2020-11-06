using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Bank.Domains.Enities;

namespace Bank.Domains.Payment.Entities
{
    public class PayOrder : BaseEntity
    {
        public long OrderNo { get; set; }

        [MaxLength(30)]
        public string FlowNo { get; set; }

        [MaxLength(20)]
        [Description("来源平台标识")]
        public string Tag { get; set; }

        public PayTypeEnum PayType { get; set; }

        public ChannelEnum Channel { get; set; }

        [MaxLength(50)]
        [Description("付款方")]
        public string Payer { get; set; }

        [MaxLength(50)]
        [Description("收款方")]
        public string Payee { get; set; }

        public long Amount { get; set; }

        public CurrencyEnum Currency { get; set; }

        [MaxLength(200)]
        [Description("支付事由")]
        public string Note { get; set; }

        public int Status { get; set; }

        [MaxLength(200)]
        public string Reserve { get; set; }

        [Description("支付时间")]
        public DateTime? PayTime { get; set; }

        [Description("交易完成时间")]
        public DateTime? EndTime { get; set; }

        public long? CloseId { get; set; }

        [Description("交易关闭时间")]
        public DateTime? CloseTime { get; set; }

        [MaxLength(200)]
        public string ErrMsg { get; set; }
    }
}
