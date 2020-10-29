using System.Collections.Generic;
using CPTech.Payment.WeChatPay.Utility;

namespace CPTech.Payment.WeChatPay.Request
{
    /// <summary>
    /// 小程序调起支付
    /// </summary>
    public class WeChatPayLiteAppSdkRequest : IWeChatPayLiteAppSdkRequest
    {
        /// <summary>
        /// 扩展字段
        /// </summary>
        public string Package { get; set; }

        public IDictionary<string, string> GetParameters()
        {
            var parameters = new WeChatPayDictionary
            {
                { "package", Package }
            };
            return parameters;
        }

        public void PrimaryHandler(WeChatPayOptions options, WeChatPayDictionary sortedTxtParams)
        {
            sortedTxtParams.Add("timeStamp", WeChatPayUtility.GetTimeStamp());
            sortedTxtParams.Add("nonceStr", WeChatPayUtility.GenerateNonceStr());
            sortedTxtParams.Add("appId", options.AppId);
            sortedTxtParams.Add("signType", WeChatPaySignType.MD5.ToString());

            sortedTxtParams.Add("paySign", WeChatPaySignature.SignWithKey(sortedTxtParams, options.Key, WeChatPaySignType.MD5));
        }
    }
}
