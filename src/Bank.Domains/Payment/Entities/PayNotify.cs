using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using CPTech.EFCore.Entities;

namespace Bank.Domains.Payment.Entities
{
    public class PayNotify : BaseEntity
    {
        [MaxLength(20)]
        [Description("来源平台标识")]
        public string Tag { get; set; }

        [MaxLength(200)]
        public string Url { get; set; }
    }
}
