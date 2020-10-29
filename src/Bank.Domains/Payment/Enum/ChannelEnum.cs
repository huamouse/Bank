using System.ComponentModel;

namespace Bank.Domains.Payment
{
    /// <summary>
    /// 支付通道
    /// </summary>
    public enum ChannelEnum
    {
        Unknown = 0,
        ICBC = 0,
        ABC,
        BOC,
        CCB
    }
}
