using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CPTech.EFCore.Entities;

namespace Bank.Domains.Payment.Entities
{
    public class PayOrderLog : BaseEntity
    {
        public long OrderNo { get; set; }

        [MaxLength(20)]
        public string FlowNo { get; set; }

        [MaxLength(20)]
        [Description("来源平台标识")]
        public string Tag { get; set; }

        public PayTypeEnum PayType { get; set; }

        public ChannelEnum Channel { get; set; }

        public string Request { get; set; }

        public string Response { get; set; }

        [MaxLength(20)]
        public string ReturnCode { get; set; }

        [MaxLength(200)]
        public string ReturnMsg { get; set; }
    }
}
