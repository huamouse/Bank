using System.ComponentModel;

namespace Bank.Domains.Payment
{
    /// <summary>
    /// 支付类型
    /// </summary>
    public enum PayTypeEnum
    {
        [Description("支付宝")]
        Alipay = 1,
        [Description("微信支付")]
        WechatPay,
        [Description("其他")]
        Other
    }
}
