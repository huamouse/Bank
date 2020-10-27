using System.ComponentModel;

namespace Bank.Domains.Payment
{
    public class OrderQueryDto
    {
        // 订单Id
        public long OrderId { get; set; }

        [Description("查询类型")]
        public string QueryType { get; set; }
    }
}
