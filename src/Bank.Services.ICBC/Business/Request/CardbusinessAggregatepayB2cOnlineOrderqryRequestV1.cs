using System;
using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class CardbusinessAggregatepayB2cOnlineOrderqryRequestV1 : IcbcRequest<CardbusinessAggregatepayB2cOnlineOrderqryResponseV1>
    {
        public override HttpMethod Method => HttpMethod.Post;

        public override string ServiceUrl => "https://gw.open.icbc.com.cn/api/cardbusiness/aggregatepay/b2c/online/orderqry/V1";

        public override Type GetBizContentClass() => typeof(CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz);

        public override Type GetResponseClass() => typeof(CardbusinessAggregatepayB2cOnlineOrderqryResponseV1);

        public class CardbusinessAggregatepayB2cOnlineOrderqryRequestV1Biz : BizContent
        {
            [JsonPropertyName("mer_id")]
            public string MerId { get; set; }

            [JsonPropertyName("out_trade_no")]
            public string OutTradeNo { get; set; }

            [JsonPropertyName("order_id")]
            public string OrderId { get; set; }

            [JsonPropertyName("deal_flag")]
            public string DealFlag { get; set; }

            [JsonPropertyName("icbc_appid")]
            public string IcbcAppId { get; set; }
        }
    }
}
