using System;

namespace Icbc.Business
{
    public class MybankPayCpayMemberapplyRequestV1 : IcbcRequest<MybankPayCpayMemberapplyResponseV1>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/mybank/pay/cpay/memberapply/V1";
        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/mybank/pay/cpay/memberapply/V1";

        public override Type GetBizContentClass() => typeof(MybankPayCpayMemberapplyRequestV1Biz);

        public override Type GetResponseClass() => typeof(MybankPayCpayMemberapplyResponseV1);

        public class MybankPayCpayMemberapplyRequestV1Biz : BizContent
        {
            public string AgreeCode { get; set; }
            public string MemberNo { get; set; }
            public string MemberName { get; set; }
            public string MemberType { get; set; }
            public string MemberRole { get; set; }
            public string CertificateType { get; set; }
            public string CertificateId { get; set; }
            public string CorpRepreName { get; set; }
            public string CorpRepreIdType { get; set; }
            public string CorpRepreId { get; set; }
            public string CorpRepreSignDate { get; set; }
            public string DealName { get; set; }
            public string DealTelphoneNo { get; set; }
            public string MallUrl { get; set; }
            public string IcpCode { get; set; }
            public string SingNoNoteAmtd { get; set; }
            public string EnterAmtType { get; set; }
            public string AccBankFlag { get; set; }
            public string Accno { get; set; }
            public string AccName { get; set; }
            public string AccBankNo { get; set; }
            public string AccBankName { get; set; }
            public string MerEname { get; set; }
            public string MerShname { get; set; }
            public string SaleDepName { get; set; }
            public string ShopAddr { get; set; }
            public string PostCode { get; set; }
            public string LinkCode { get; set; }
            public string EMail { get; set; }
            public string RegAmt { get; set; }
            public string CallbackUrl { get; set; }
        }
    }
}
