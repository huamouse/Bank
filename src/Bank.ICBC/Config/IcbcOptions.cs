using System.Collections.Generic;

namespace Bank.ICBC.Config
{
    public sealed class IcbcOptions
    {
        public string AppId { get; set; }
        public string PrivateKey { get; set; }
        public string GatewayPublicKey { get; set; }
        public List<MerInfo> MerInfos { get; set; }

        public class MerInfo
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string PrtclNo { get; set; }
        }
    }
}
