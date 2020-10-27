using System.Collections.Generic;

namespace CPTech.Payment.WeChatPay.Request
{
    /// <summary>
    /// 小程序调起支付
    /// </summary>
    public class WeChatLoginRequest : IWeChatLoginRequest
    {
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string JsCode { get; set; }

        public string GetRequestUrl(WeChatPayOptions options)
        {
            return $"https://api.weixin.qq.com/sns/jscode2session?appid={options.AppId}&secret={options.AppSecret}&js_code={JsCode}&grant_type=authorization_code";
        }
    }
}
