using System;

namespace Icbc.Business
{
    public class MybankPayCpayCppreservationpayRequestV2 : IcbcRequest<MybankPayCpayCppreservationpayResponseV2>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/mybank/pay/cpay/cppreservationpay/V2";
        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/mybank/pay/cpay/cppreservationpay/V2";

        public override Type GetBizContentClass() => typeof(MybankPayCpayCppreservationpayRequestV2Biz);

        public override Type GetResponseClass() => typeof(MybankPayCpayCppreservationpayResponseV2);

        public class MybankPayCpayCppreservationpayRequestV2Biz : BizContent
        {
            public string AgreeCode { get; set; }

            public string OrderCode { get; set; }

            public string PartnerSeq { get; set; }

            public string PartnerSeqOrigin { get; set; }

            public string PayAmount { get; set; }

            public string OrderCurr { get; set; }

            public string PayeeSysflag { get; set; }

            public string PayeeAccno { get; set; }

            public string PayeeCompanyName { get; set; }

            public string PayeeBankCode { get; set; }

            public string SubmitTime { get; set; }

            public string OrderRemark { get; set; }

            public string ReceiptRemark { get; set; }

            public string Purpose { get; set; }

            public string Summary { get; set; }

            public string OperType { get; set; }

            public string PayerMemberNo { get; set; }

            public string PayerMemberName { get; set; }

            public string Note { get; set; }

            public string CrmemberNo { get; set; }

            public string CrmemberName { get; set; }

            public string TradeName { get; set; }

            public string TradeNum { get; set; }

            public string TradeUnit { get; set; }
        }
    }
}
