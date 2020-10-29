using System.ComponentModel;

namespace Bank.Domains.Payment
{
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayTypeEnum
    {
        [Description("未知")]
        Unknown = 0,
        [Description("支付宝")]
        Alipay,
        [Description("微信支付")]
        WechatPay,
        [Description("银联")]
        Bank
    }
}
