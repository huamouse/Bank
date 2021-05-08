using System;

namespace Icbc.Business
{
    public class MybankPayCpayCpordercloseRequestV1 : IcbcRequest<MybankPayCpayCpordercloseResponseV1>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/mybank/pay/cpay/cporderclose/V1";
        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/mybank/pay/cpay/cporderclose/V1";

        public override Type GetBizContentClass() => typeof(MybankPayCpayCpordercloseV1RequestV1Biz);

        public override Type GetResponseClass() => typeof(MybankPayCpayCpordercloseResponseV1);

        public class MybankPayCpayCpordercloseV1RequestV1Biz : BizContent
        {
            public string AgreeCode { get; set; }

            public string PartnerSeq { get; set; }
            public string OrderCode { get; set; }
        }
    }
}
