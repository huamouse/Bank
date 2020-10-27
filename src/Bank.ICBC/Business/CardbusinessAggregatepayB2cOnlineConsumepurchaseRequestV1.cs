using System;
using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1 : IcbcRequest<CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1>
    {
        public override HttpMethod Method => HttpMethod.Post;

        //public override string ServiceUrl => "https://apipcs3.dccnet.com.cn/api/cardbusiness/aggregatepay/b2c/online/consumepurchase/V1";
        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/cardbusiness/aggregatepay/b2c/online/consumepurchase/V1";

        public override Type GetBizContentClass() => typeof(CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz);

        public override Type GetResponseClass() => typeof(CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1);

        public class CardbusinessAggregatepayB2cOnlineConsumepurchaseRequestV1Biz : BizContent
        {
            [JsonPropertyName("mer_id")]
            public string MerId { get; set; }

            [JsonPropertyName("out_trade_no")]
            public string OutTradeNo { get; set; }

            [JsonPropertyName("pay_mode")]
            public string PayMode { get; set; }

            [JsonPropertyName("access_type")]
            public string AccessType { get; set; }

            [JsonPropertyName("mer_prtcl_no")]
            public string MerPrtclNo { get; set; }

            [JsonPropertyName("orig_date_time")]
            public string OrigDateTime { get; set; }

            [JsonPropertyName("decive_info")]
            public string DeciveInfo { get; set; }

            public string Body { get; set; }

            [JsonPropertyName("fee_type")]
            public string FeeType { get; set; }

            [JsonPropertyName("spbill_create_ip")]
            public string SpbillCreateIp { get; set; }

            [JsonPropertyName("total_fee")]
            public string TotalFee { get; set; }

            [JsonPropertyName("mer_url")]
            public string MerUrl { get; set; }

            [JsonPropertyName("shop_appid")]
            public string ShopAppid { get; set; }

            [JsonPropertyName("icbc_appid")]
            public string IcbcAppId { get; set; }

            [JsonPropertyName("open_id")]
            public string OpenId { get; set; }

            [JsonPropertyName("union_id")]
            public string UnionId { get; set; }

            [JsonPropertyName("mer_acct")]
            public string MerAcct { get; set; }

            [JsonPropertyName("expire_time")]
            public string ExpireTime { get; set; }

            public string Attach { get; set; }

            [JsonPropertyName("notify_type")]
            public string NotifyType { get; set; }

            [JsonPropertyName("result_type")]
            public string ResultType { get; set; }

            [JsonPropertyName("pay_limit")]
            public string PayLimit { get; set; }

            [JsonPropertyName("order_apd_inf")]
            public string OrderApdInf { get; set; }

        }
    }
}