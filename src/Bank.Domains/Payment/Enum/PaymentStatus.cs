using System.ComponentModel;

namespace Bank.Domains.Payment
{
    public enum PaymentStatus
    {
        [Description("未支付")]
        Unpaid,
        [Description("支付中")]
        Paying,
        [Description("支付失败")]
        Fail,
        [Description("支付成功")]
        Success,
        [Description("交易关闭")]
        Closed = 9,
        [Description("交易取消")]
        Canceled
    }
}
