using System.Collections.Generic;

namespace Icbc.Settings
{
    public sealed class IcbcOptions
    {
        public string AppId { get; set; }
        public string WeixinAppId { get; set; }
        public string PrivateKey { get; set; }
        public string PrivateKeyType { get; set; }
        public string GatewayPublicKey { get; set; }
        public string NotifyUrl { get; set; }
        public List<MerInfo> MerInfos { get; set; }

        public string BizAppId { get; set; }
        public string BizPrivateKey { get; set; }
        public string BizPrivateKeyType { get; set; }
        public string BizGatewayPublicKey { get; set; }
        public string ReturnUrl { get; set; }
        public string BizNotifyUrl { get; set; }
        public List<MerInfo> BizMerInfos { get; set; }

        public class MerInfo
        {
            public string Id { get; set; }
            public string AccNo { get; set; }
            public string AccName { get; set; }
            public string AccBankNo { get; set; }
            public string AccBankName { get; set; }
            public string IcbcFlag { get; set; }
            public string PrtclNo { get; set; }
            public string PayMode { get; set; }
            public string CertificateNo { get; set; }
            public string CorporationName { get; set; }
            public string CorporationNo { get; set; }
            public string DealName { get; set; }
            public string DealTelNo { get; set; }
            public string IcpCode { get; set; }
        }
    }
}
