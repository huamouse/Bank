using System.Collections.Generic;

namespace Icbc.Business
{
    public class MybankPayCpayMemberapplyResponseV1 : IcbcResponse
    {
        public int TransOk { get; set; }
        public int TotalNum { get; set; }
        public List<MemInfo> MemInfoList { get; set; }

        public class MemInfo
        {
            public string AgreeCode { get; set; }
            public string MemberNo { get; set; }
            public string Status { get; set; }
            public string CardNo { get; set; }
            public string Erecstat { get; set; }
            public string Jrecstat { get; set; }
        }
    }
}
