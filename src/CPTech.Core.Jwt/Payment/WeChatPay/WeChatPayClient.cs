using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CPTech.Payment.Security;
using CPTech.Payment.WeChatPay.Notify;
using CPTech.Payment.WeChatPay.Parser;
using CPTech.Payment.WeChatPay.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using MD5 = CPTech.Payment.Security.MD5;

namespace CPTech.Payment.WeChatPay
{
    public class WeChatPayClient : IWeChatPayClient
    {
        public const string Prefix = nameof(WeChatPayClient) + ".";

        private readonly IHttpClientFactory httpClientFactory;
        private readonly ILogger<WeChatPayClient> logger;

        public WeChatPayClient(IHttpClientFactory httpClientFactory, ILogger<WeChatPayClient> logger)
        {
            this.httpClientFactory = httpClientFactory;
            this.logger = logger;
        }

        public async Task<string>GetAsync(string url)
        {
            var client = httpClientFactory.CreateClient(nameof(WeChatPayClient));
            var body = await client.GetAsync(url);

            return await body.Content.ReadAsStringAsync();
        }

        public async Task<T> ExecuteAsync<T>(IWeChatPayRequest<T> request, WeChatPayOptions options) where T : WeChatPayResponse
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.AppId)) throw new ArgumentNullException(nameof(options.AppId));
            if (string.IsNullOrEmpty(options.MchId)) throw new ArgumentNullException(nameof(options.MchId));
            if (string.IsNullOrEmpty(options.Key)) throw new ArgumentNullException(nameof(options.Key));

            var signType = request.GetSignType();
            var sortedTxtParams = new WeChatPayDictionary(request.GetParameters());
            request.PrimaryHandler(options, signType, sortedTxtParams);

            var client = httpClientFactory.CreateClient(nameof(WeChatPayClient));
            var body = await client.PostAsync(request.GetRequestUrl(), sortedTxtParams);
            var parser = new WeChatPayXmlParser<T>();
            var response = parser.Parse(body);

            if (request.GetNeedCheckSign()) CheckResponseSign(response, options, signType);

            return response;
        }

        public Task<T> PageExecuteAsync<T>(IWeChatPayRequest<T> request, WeChatPayOptions options) where T : WeChatPayResponse
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.AppId)) throw new ArgumentNullException(nameof(options.AppId));
            if (string.IsNullOrEmpty(options.MchId)) throw new ArgumentNullException(nameof(options.MchId));
            if (string.IsNullOrEmpty(options.Key)) throw new ArgumentNullException(nameof(options.Key));

            var signType = request.GetSignType();
            var sortedTxtParams = new WeChatPayDictionary(request.GetParameters());

            request.PrimaryHandler(options, signType, sortedTxtParams);

            var url = request.GetRequestUrl();
            if (url.Contains("?"))
                url += "&" + WeChatPayUtility.BuildQuery(sortedTxtParams);
            else
                url += "?" + WeChatPayUtility.BuildQuery(sortedTxtParams);

            var rsp = Activator.CreateInstance<T>();
            rsp.Body = url;
            return Task.FromResult(rsp);
        }

        public async Task<JsonDocument> ExecuteAsync(IWeChatLoginRequest request, WeChatPayOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.AppId)) throw new ArgumentNullException(nameof(options.AppId));
            if (string.IsNullOrEmpty(options.AppSecret)) throw new ArgumentNullException(nameof(options.AppSecret));

            var client = httpClientFactory.CreateClient(nameof(WeChatPayClient));
            string url = request.GetRequestUrl(options);
            logger.LogInformation("request url:{url}");
            var body = await client.GetAsync(url);

            return JsonDocument.Parse(await body.Content.ReadAsStringAsync());
        }

        public Task<WeChatPayDictionary> ExecuteAsync(IWeChatPayLiteAppSdkRequest request, WeChatPayOptions options)
        {
            _ = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.AppId)) throw new ArgumentNullException(nameof(options.AppId));
            if (string.IsNullOrEmpty(options.MchId)) throw new ArgumentNullException(nameof(options.MchId));
            if (string.IsNullOrEmpty(options.Key)) throw new ArgumentNullException(nameof(options.Key));

            var sortedTxtParams = new WeChatPayDictionary(request.GetParameters());
            request.PrimaryHandler(options, sortedTxtParams);

            return Task.FromResult(sortedTxtParams);
        }

        private void CheckResponseSign(WeChatPayResponse response, WeChatPayOptions options, WeChatPaySignType signType)
        {
            if (string.IsNullOrEmpty(response.Body)) throw new WeChatPayException("sign check fail: Body is Empty!");

            if (response.Parameters.Count == 0) throw new WeChatPayException("sign check fail: Parameters is Empty!");

            if (response.Parameters["return_code"] == "SUCCESS")
            {
                if (!response.Parameters.TryGetValue("sign", out var sign)) throw new WeChatPayException("sign check fail: sign is Empty!");

                var cal_sign = WeChatPaySignature.SignWithKey(response.Parameters, options.Key, signType);
                if (cal_sign != sign) throw new WeChatPayException("sign check fail: check Sign and Data Fail!");
            }
        }

        public async Task<T> ExecuteAsync<T>(HttpRequest request, WeChatPayOptions options) where T : WeChatPayNotify
        {
            _ = request ?? throw new ArgumentNullException(nameof(request));
            _ = options ?? throw new ArgumentNullException(nameof(options));
            if (string.IsNullOrEmpty(options.Key)) throw new ArgumentNullException(nameof(options.Key));

            var body = await new StreamReader(request.Body, Encoding.UTF8).ReadToEndAsync();
            var parser = new WeChatPayXmlParser<T>();
            var notify = parser.Parse(body);
            if (notify is WeChatPayRefundNotify)
            {
                var key = MD5.Compute(options.Key).ToLowerInvariant();
                var data = AES.Decrypt((notify as WeChatPayRefundNotify).ReqInfo, key, CipherMode.ECB, PaddingMode.PKCS7);
                notify = parser.Parse(body, data);
            }
            else
                CheckNotifySign(notify, options);

            return notify;
        }

        private void CheckNotifySign(WeChatPayNotify notify, WeChatPayOptions options)
        {
            if (string.IsNullOrEmpty(notify.Body)) throw new WeChatPayException("sign check fail: Body is Empty!");
            if (notify.Parameters.Count == 0) throw new WeChatPayException("sign check fail: Parameters is Empty!");
            if (!notify.Parameters.TryGetValue("sign", out var sign)) throw new WeChatPayException("sign check fail: sign is Empty!");

            var cal_sign = WeChatPaySignature.SignWithKey(notify.Parameters, options.Key, WeChatPaySignType.MD5);
            if (cal_sign != sign) throw new WeChatPayException("sign check fail: check Sign and Data Fail!");
        }
    }
}
