using System.Collections.Generic;

namespace CPTech.Payment.WeChatPay
{
    public interface IWeChatLoginRequest
    {
        string GetRequestUrl(WeChatPayOptions options);
    }
}
