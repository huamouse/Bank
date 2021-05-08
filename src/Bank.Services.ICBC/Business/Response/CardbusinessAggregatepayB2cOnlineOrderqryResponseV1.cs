using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class CardbusinessAggregatepayB2cOnlineOrderqryResponseV1 : IcbcResponse
    {
        [JsonPropertyName("pay_status")]
        public string PayStatus { get; set; }

        [JsonPropertyName("card_no")]
        public string CardNo { get; set; }

        [JsonPropertyName("mer_id")]
        public string MerId { get; set; }

        [JsonPropertyName("total_amt")]
        public string TotalAmt { get; set; }

        [JsonPropertyName("point_amt")]
        public string PointAmt { get; set; }

        [JsonPropertyName("ecoupon_amt")]
        public string EcouponAmt { get; set; }

        [JsonPropertyName("mer_disc_amt")]
        public string MerDiscAmt { get; set; }

        [JsonPropertyName("coupon_amt")]
        public string CouponAmt { get; set; }

        [JsonPropertyName("bank_disc_amt")]
        public string BankDiscAmt { get; set; }

        [JsonPropertyName("payment_amt")]
        public string PaymentAmt { get; set; }

        [JsonPropertyName("out_trade_no")]
        public string OutTradeNo { get; set; }

        [JsonPropertyName("order_id")]
        public string OrderId { get; set; }

        [JsonPropertyName("pay_time")]
        public string PayTime { get; set; }

        [JsonPropertyName("total_disc_amt")]
        public string TotalDiscAmt { get; set; }

        [JsonPropertyName("attach")]
        public string Attach { get; set; }

        [JsonPropertyName("third_trade_no")]
        public string ThirdTradeNo { get; set; }

        [JsonPropertyName("third_party_return_code")]
        public string ThirdPartyReturnCode { get; set; }

        [JsonPropertyName("third_party_return_msg")]
        public string ThirdPartyReturnMsg { get; set; }

        [JsonPropertyName("card_flag")]
        public string CardFlag { get; set; }

        [JsonPropertyName("decr_flag")]
        public string DecrFlag { get; set; }

        [JsonPropertyName("open_id")]
        public string OpenId { get; set; }

        [JsonPropertyName("pay_type")]
        public string PayType { get; set; }

        [JsonPropertyName("card_kind")]
        public string CardKind { get; set; }

        [JsonPropertyName("access_type")]
        public string AccessType { get; set; }
    }
}
