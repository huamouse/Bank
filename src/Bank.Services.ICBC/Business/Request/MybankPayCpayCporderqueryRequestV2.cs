using System;

namespace Icbc.Business
{
    public class MybankPayCpayCporderqueryRequestV2 : IcbcRequest<MybankPayCpayCporderqueryResponseV2>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/mybank/pay/cpay/cporderquery/V2";
        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/mybank/pay/cpay/cporderquery/V2";

        public override Type GetBizContentClass() => typeof(QueryPayApplyRequestV2Biz);

        public override Type GetResponseClass() => typeof(MybankPayCpayCporderqueryResponseV2);

        public class QueryPayApplyRequestV2Biz : BizContent
        {
            public string AgreeCode { get; set; }

            public string PartnerSeq { get; set; }

            public string OrderCode { get; set; }
        }
    }
}
