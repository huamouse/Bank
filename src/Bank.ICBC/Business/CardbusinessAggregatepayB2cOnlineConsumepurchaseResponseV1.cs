using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class CardbusinessAggregatepayB2cOnlineConsumepurchaseResponseV1 : IcbcResponse
    {
        [JsonPropertyName("total_amt")]
        public string TotalAmt { get; set; }

        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("pay_time")]
        public string PayTime { get; set; }

        [JsonPropertyName("mer_id")]
        public string MerId { get; set; }

        [JsonPropertyName("pay_mode")]
        public string PayMode { get; set; }

        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }

        [JsonPropertyName("card_kind")]
        public string CardKind { get; set; }

        [JsonPropertyName("trade_type")]
        public string TradeType { get; set; }

        [JsonPropertyName("wx_data_package")]
        public string WxDataPackage { get; set; }

        [JsonPropertyName("zfb_data_package")]
        public string ZfbDataPackage { get; set; }

        [JsonPropertyName("third_party_return_code")]
        public string ThirdPartyReturnCode { get; set; }

        [JsonPropertyName("third_party_return_msg")]
        public string ThirdPartyReturnMsg { get; set; }

    }
}
