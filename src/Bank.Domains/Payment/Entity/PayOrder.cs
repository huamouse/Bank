using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bank.Domains.Payment
{
    public class PayOrder
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        public long OrderNo { get; set; }

        [MaxLength(20)]
        public string FlowNo { get; set; }

        [MaxLength(20)]
        [Description("来源平台标识")]
        public string Source { get; set; }

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
        public DateTime PayTime { get; set; }

        [Description("交易完成时间")]
        public DateTime EndTime { get; set; }

        [MaxLength(50)]
        public string CreatorId { get; set; }

        public DateTime CreationTime { get; set; }

        [MaxLength(50)]
        public string CloseId { get; set; }

        [Description("交易关闭时间")]
        public DateTime CloseTime { get; set; }

        public bool IsDeleted { get; set; }

        [MaxLength(200)]
        public string ErrMsg { get; set; }
    }
}
