using System.ComponentModel;

namespace Bank.Domains.Payment
{
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum QueryTypeEnum
    {
        [Description("按订单号查询")]
        OrderQuery,
        [Description("按银行流水号查询")]
        FlowQuery,
        [Description("关闭请求")]
        Close = 9
    }
}
