using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class MybankPayCpayCppayapplyRequestV2 : IcbcRequest<MybankPayCpayCppayapplyResponseV2>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/mybank/pay/cpay/cppayapply/V2";
        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/mybank/pay/cpay/cppayapply/V2";

        public override Type GetBizContentClass() => typeof(MybankPayCpayCppayapplyResponseV2);

        public override Type GetResponseClass() => typeof(MybankPayCpayCppayapplyResponseV2);

        public class RecvMallInfo : BizContent
        {
            [JsonPropertyName("mallCode")]
            public string MallCode { get; set; }
            [JsonPropertyName("mccCode")]
            public string MccCode { get; set; }
            [JsonPropertyName("mccName")]
            public string MccName { get; set; }
            [JsonPropertyName("businessLicense")]
            public string BusinessLicense { get; set; }
            [JsonPropertyName("businessLicenseType")]
            public string BusinessLicenseType { get; set; }
            [JsonPropertyName("mallName")]
            public string MallName { get; set; }
            [JsonPropertyName("payeeCompanyName")]
            public string PayeeCompanyName { get; set; }
            [JsonPropertyName("payeeSysflag")]
            public string PayeeSysflag { get; set; }
            [JsonPropertyName("payeeBankCode")]
            public string PayeeBankCode { get; set; }
            [JsonPropertyName("payeeHeadBankCode")]
            public string PayeeHeadBankCode { get; set; }
            [JsonPropertyName("payeeAccno")]
            public string PayeeAccno { get; set; }
            [JsonPropertyName("payAmount")]
            public string PayAmount { get; set; }
            [JsonPropertyName("payeeBankCountry")]
            public string PayeeBankCountry { get; set; }
            [JsonPropertyName("rbankname")]
            public string Rbankname { get; set; }
            [JsonPropertyName("payeeBankSign")]
            public string PayeeBankSign { get; set; }
            [JsonPropertyName("payeeCountry")]
            public string PayeeCountry { get; set; }
            [JsonPropertyName("payeeAddress")]
            public string PayeeAddress { get; set; }
            [JsonPropertyName("racaddress1")]
            public string Racaddress1 { get; set; }
            [JsonPropertyName("racaddress2")]
            public string Racaddress2 { get; set; }
            [JsonPropertyName("racaddress3")]
            public string Racaddress3 { get; set; }
            [JsonPropertyName("racaddress4")]
            public string Racaddress4 { get; set; }
            [JsonPropertyName("racpostcode")]
            public string Racpostcode { get; set; }
            [JsonPropertyName("agentbic")]
            public string Agentbic { get; set; }
            [JsonPropertyName("payeePhone")]
            public string PayeePhone { get; set; }
            [JsonPropertyName("payeeOrgcode")]
            public string PayeeOrgcode { get; set; }
            [JsonPropertyName("receivableAmount")]
            public string ReceivableAmount { get; set; }
        }

        public class GoodsInfo
        {
            [JsonPropertyName("goodsSubId")]
            public string GoodsSubId { get; set; }
            [JsonPropertyName("goodsName")]
            public string GoodsName { get; set; }
            [JsonPropertyName("payeeCompanyName")]
            public string PayeeCompanyName { get; set; }
            [JsonPropertyName("goodsNumber")]
            public string GoodsNumber { get; set; }
            [JsonPropertyName("goodsUnit")]
            public string GoodsUnit { get; set; }
            [JsonPropertyName("goodsAmt")]
            public string GoodsAmt { get; set; }
        }

        public class MybankPayCpayCppayapplyRequestV2Biz : BizContent
        {
            [JsonPropertyName("agreeCode")]
            public string AgreeCode { get; set; }

            [JsonPropertyName("partnerSeq")]
            public string PartnerSeq { get; set; }

            [JsonPropertyName("payChannel")]
            public string PayChannel { get; set; }

            [JsonPropertyName("internationalFlag")]
            public string InternationalFlag { get; set; }

            [JsonPropertyName("payMode")]
            public string PayMode { get; set; }

            [JsonPropertyName("operType")]
            public string OperType { get; set; } = "301";

            [JsonPropertyName("reservDirect")]
            public string ReservDirect { get; set; }

            [JsonPropertyName("payEntitys")]
            public string PayEntitys { get; set; }

            [JsonPropertyName("asynFlag")]
            public string AsynFlag { get; set; }

            [JsonPropertyName("logonId")]
            public string LogonId { get; set; }

            [JsonPropertyName("payerAccno")]
            public string PayerAccno { get; set; }

            [JsonPropertyName("payerAccname")]
            public string PayerAccname { get; set; }

            [JsonPropertyName("payerFeeAccno")]
            public string PayerFeeAccno { get; set; }

            [JsonPropertyName("payerFeeAccName")]
            public string PayerFeeAccName { get; set; }

            [JsonPropertyName("payerFeeCurr")]
            public string PayerFeeCurr { get; set; }

            [JsonPropertyName("payMemno")]
            public string PayMemno { get; set; }

            [JsonPropertyName("orgcode")]
            public string Orgcode { get; set; }

            [JsonPropertyName("orderCode")]
            public string OrderCode { get; set; }

            [JsonPropertyName("orderAmount")]
            public string OrderAmount { get; set; }

            [JsonPropertyName("orderCurr")]
            public string OrderCurr { get; set; }

            [JsonPropertyName("sumPayamt")]
            public string SumPayamt { get; set; }

            [JsonPropertyName("orderRemark")]
            public string OrderRemark { get; set; }

            [JsonPropertyName("rceiptRemark")]
            public string RceiptRemark { get; set; }

            [JsonPropertyName("purpose")]
            public string Purpose { get; set; }

            [JsonPropertyName("summary")]
            public string Summary { get; set; }

            [JsonPropertyName("submitTime")]
            public string SubmitTime { get; set; }

            [JsonPropertyName("returnUrl")]
            public string ReturnUrl { get; set; }

            [JsonPropertyName("callbackUrl")]
            public string CallbackUrl { get; set; }

            [JsonPropertyName("agreementId")]
            public string AgreementId { get; set; }

            [JsonPropertyName("invoiceId")]
            public string InvoiceId { get; set; }

            [JsonPropertyName("manifestId")]
            public string ManifestId { get; set; }

            [JsonPropertyName("agreementImageId")]
            public string AgreementImageId { get; set; }

            [JsonPropertyName("enterpriseName")]
            public string EnterpriseName { get; set; }

            [JsonPropertyName("payRemark")]
            public string PayRemark { get; set; }

            [JsonPropertyName("bakReserve1")]
            public string BakReserve1 { get; set; }

            [JsonPropertyName("bakReserve2")]
            public string BakReserve2 { get; set; }

            [JsonPropertyName("bakReserve3")]
            public string BakReserve3 { get; set; }

            [JsonPropertyName("partnerSeqOrigin")]
            public string PartnerSeqOrigin { get; set; }

            [JsonPropertyName("sumPayamtOrigin")]
            public string SumPayamtOrigin { get; set; }

            [JsonPropertyName("reporterName")]
            public string ReporterName { get; set; }

            [JsonPropertyName("reporterContact")]
            public string ReporterContact { get; set; }

            [JsonPropertyName("identityMode")]
            public string IdentityMode { get; set; }

            [JsonPropertyName("payeeList")]
            public List<RecvMallInfo> PayeeList { get; set; }

            [JsonPropertyName("goodsList")]
            public List<GoodsInfo> GoodsList { get; set; }
        }
    }
}
