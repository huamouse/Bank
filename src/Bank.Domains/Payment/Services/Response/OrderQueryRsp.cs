using System;

namespace Bank.Domains.Payment.Services
{
    public class OrderQueryRsp : BasePayRsp
    {
        public DateTime? EndTime { get; set; }
        public decimal? Amount { get; set; }
        public string Reserve { get; set; }
    }
}
