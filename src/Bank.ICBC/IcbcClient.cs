using CPTech.Security;
using Icbc.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Icbc
{
    public class IcbcClient// : IcbcClient
    {
        protected string appId;
        protected string privateKey;
        protected string signType = "RSA2";
        protected string charset = "UTF-8";
        protected string format = "json";
        protected string gatewayPulicKey;

        private readonly HttpClient httpClient;

        public IcbcClient(string appId, string privateKey, string gatewayPulicKey, HttpClient httpClient)
        {
            this.appId = appId;
            this.privateKey = privateKey;
            this.gatewayPulicKey = gatewayPulicKey;
            this.httpClient = httpClient;
        }

        public IcbcClient(string appId, string signType, string privateKey, string charset, string format, string gatewayPulicKey)
        {
            this.appId = appId;
            this.signType = signType;
            this.privateKey = privateKey;
            this.charset = charset;
            this.format = format;
            this.gatewayPulicKey = gatewayPulicKey;
        }

        public IcbcResponse Execute<T>(IcbcRequest<T> request) where T : IcbcResponse
        {
            return Execute(request, Guid.NewGuid().ToString().Replace("-", ""), "");
        }

        public IcbcResponse Execute<T>(IcbcRequest<T> request, string msgId, string appAuthToken) where T : IcbcResponse
        {
            Dictionary<string, string> param = PrepareParams(request, msgId, appAuthToken);
            string respStr;

            switch (request.Method)
            {
                case HttpMethod.Get:
                    respStr = httpClient.GetStringAsync(request.ServiceUrl).GetAwaiter().GetResult();
                    //respStr = WebUtil.GetHttpResponseStr(request.ServiceUrl, param, charset);
                    break;
                case HttpMethod.Post:
                    //HttpContent httpContent = new ByteArrayContent(ToByteArrayContent(param, charset));
                    HttpContent httpContent = new FormUrlEncodedContent(param);
                    respStr = httpClient.PostAsync(request.ServiceUrl, httpContent).GetAwaiter().GetResult().Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    //respStr = WebUtil.GetResponseJson(request.ServiceUrl, param, charset);
                    break;
                default:
                    throw new Exception("only support GET or POST, method: " + request.Method.ToString());
            }

            IcbcResponse response = ParseJsonWithIcbcSign(request, respStr) ?? throw new Exception("response is null.");

            return response;
        }

        protected Dictionary<string, string> PrepareParams<T>(IcbcRequest<T> request, string msgId, string appAuthToken) where T : IcbcResponse
        {
            string bizContentStr = BuildBizContentStr(request);
            string path = string.Empty;
            try
            {
                path = new Uri(request.ServiceUrl).LocalPath;
            }
            catch (UriFormatException e)
            {
                Console.WriteLine(e.Message);
            }

            Dictionary<string, string> param = new Dictionary<string, string>();

            Dictionary<string, string> extraParams = request.ExtraParams;
            if (extraParams != null) param = param.Union(extraParams).ToDictionary(k => k.Key, v => v.Value);

            param.Add(IcbcConstants.APP_ID, appId);
            param.Add(IcbcConstants.SIGN_TYPE, signType);
            param.Add(IcbcConstants.CHARSET, charset);
            param.Add(IcbcConstants.FORMAT, format);
            //param.Add(IcbcConstants.CA, null);
            param.Add(IcbcConstants.APP_AUTH_TOKEN, appAuthToken);
            param.Add(IcbcConstants.MSG_ID, msgId);
            param.Add(IcbcConstants.TIMESTAMP, DateTime.Now.ToString(IcbcConstants.DATE_TIME_FORMAT));

            if (request.IsNeedEncrypt)
            {
                //if (encryptType == null || encryptKey == null)
                //{
                //    throw new ArgumentNullException("request need be encrypted, encrypt type and encrypt key can not be null.");
                //}
                //if (bizContentStr != null)
                //{
                //    param.Add(IcbcConstants.ENCRYPT_TYPE, encryptType);
                //    param.Add(IcbcConstants.BIZ_CONTENT_KEY, IcbcSignature.EncryptContent(bizContentStr, encryptType, encryptKey, charset));
                //}
            }
            else
            {
                param.Add(IcbcConstants.BIZ_CONTENT_KEY, bizContentStr);
            }
            string strToSign = WebUtil.BuildOrderedSignStr(path, param);
            RsaUtil rsaUtil = new RsaUtil(privateKey, null);
            string signedStr = rsaUtil.Sign(strToSign);
            //String signedStr = IcbcSignature.Sign(strToSign, signType, privateKey, charset, null);
            if (signedStr.Length < 3)
                throw new Exception("sign Exception!");

            param.Add(IcbcConstants.SIGN, signedStr);

            return param;
        }

        protected string BuildBizContentStr<T>(IcbcRequest<T> request) where T : IcbcResponse
        {
            if (request.Content == null) return null;

            if (string.Equals(IcbcConstants.FORMAT_JSON, format))
            {
                return JsonSerializer.Serialize(request.Content, request.Content.GetType(), new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    IgnoreNullValues = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
            }

            return null;
        }

        private IcbcResponse ParseJsonWithIcbcSign<T>(IcbcRequest<T> request, string respStr) where T : IcbcResponse
        {
            string respBizContentStr = string.Empty;
            string sign = string.Empty;
            int indexOfRootStart = respStr.IndexOf(IcbcConstants.RESPONSE_BIZ_CONTENT)
                    + IcbcConstants.RESPONSE_BIZ_CONTENT.Length + 2;
            int indexOfRootEnd = respStr.LastIndexOf(",\"sign\":\"");
            int indexOfSignStart = indexOfRootEnd + IcbcConstants.SIGN.Length + 5;
            int indexOfSignEnd = respStr.LastIndexOf("\"");
            respBizContentStr = respStr.Substring(indexOfRootStart, indexOfRootEnd - indexOfRootStart);
            sign = respStr.Substring(indexOfSignStart, indexOfSignEnd - indexOfSignStart);

            RsaUtil rsaUtil = new RsaUtil(null, gatewayPulicKey, RSAType.RSA);
            bool passed = rsaUtil.Verify(respBizContentStr, sign);
            
            if (!passed)
            {
                throw new Exception("icbc sign verify not passed.");
            }
            if (request.IsNeedEncrypt)
            {
                //解密【目前仅支持AES加解密方法】
                //respBizContentStr = IcbcSignature.DecryptContent(respBizContentStr.Substring(1, respBizContentStr.Length - 1), encryptType, encryptKey, charset);

            }
            //反序列化并返回
            IcbcResponse response = null;
            //MemoryStream ms1 = new MemoryStream(Encoding.GetEncoding(charset).GetBytes(respBizContentStr));
            //using (MemoryStream ms = new MemoryStream(Encoding.GetEncoding(charset).GetBytes(respBizContentStr)))
            //{
            //    DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(request.getResponseClass());
            //    response = (IcbcResponse)deseralizer.ReadObject(ms) as IcbcResponse;
            //}
            response = JsonSerializer.Deserialize<T>(respBizContentStr);
            //response = (IcbcResponse)JsonConvert.DeserializeObject(respBizContentStr, request.getResponseClass());
            return response;
        }

        private byte[] ToByteArrayContent(IDictionary<string, string> parameters, string charset)
        {
            Encoding encode = Encoding.GetEncoding(charset);
            StringBuilder buffer = new StringBuilder();
            if (!(parameters == null || parameters.Count == 0))
            {
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (parameters[key] == null || parameters[key].Equals(""))
                    {
                        continue;
                    }
                    else
                    {
                        if (i > 0)
                            buffer.AppendFormat("&{0}={1}", key, HttpUtility.UrlEncode(parameters[key], Encoding.GetEncoding(charset)));
                        else
                            buffer.AppendFormat("{0}={1}", key, HttpUtility.UrlEncode(parameters[key], Encoding.GetEncoding(charset)));
                    }
                    i++;
                }
            }

            return encode.GetBytes(buffer.ToString());
        }
    }
}