using System.Text.Json.Serialization;

namespace Bank.Domains.Payment
{
    public class B2cNotifyDto
    {
        public string From { get; set; }
        public string Api { get; set; }
        public string App_id { get; set; }
        public string Charset { get; set; }
        public string Format { get; set; }
        public string Encrypt_type { get; set; }
        public string Timestamp { get; set; }
        public string Sign_type { get; set; }
        public string Sign { get; set; }
        public string Biz_content { get; set; }
        public class BizContent
        {
            [JsonPropertyName("return_code")]
            public string ReturnCode { get; set; }
            [JsonPropertyName("return_msg")]
            public string ReturnMsg { get; set; }
            [JsonPropertyName("msg_id")]
            public string MsgId { get; set; }
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
        }
    }
}
