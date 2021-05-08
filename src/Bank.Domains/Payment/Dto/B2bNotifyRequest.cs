using System.Text.Json.Serialization;

namespace Bank.Domains.Payment
{
    public class B2bNotifyDto
    {
        public string From { get; set; }
        public string Api { get; set; }
        public string Appid { get; set; }
        public string Charset { get; set; }
        public string Format { get; set; }
        public string Encrypt_type { get; set; }
        public string Timestamp { get; set; }
        public string Biz_content { get; set; }
        public string Sign_type { get; set; }
        public string Sign { get; set; }
        public class BizContent
        {
            [JsonPropertyName("app_id")]
            public string AppId { get; set; }
            public string SerialNo { get; set; }
            public string AgreeCode { get; set; }
            public string AgreeName { get; set; }
            public string PartnerSeq { get; set; }
            public int PayMode { get; set; }
            public int PayType { get; set; }
            public string OrderAmount { get; set; }
            public string OrderCurr { get; set; }
            public int SumPayamt { get; set; }
            public string Source { get; set; }
            public string AccrualCny { get; set; }
            public string AccrualForeign { get; set; }
            public string PayerAccno { get; set; }
            public string PayerAccname { get; set; }
            public string PayerAccnoForeign { get; set; }
            public string PayerAccnameForeign { get; set; }
            public string PayStatus { get; set; }
            //public string PayPlanList { get; set; }
            //public string PayeeList { get; set; }
            public string PayerWalletId { get; set; }
            public string PayerWalletName { get; set; }
            public string PayerBankName { get; set; }
            public string PayerBankCode { get; set; }
            [JsonPropertyName("return_code")]
            public string ReturnCode { get; set; }
            [JsonPropertyName("return_msg")]
            public string ReturnMsg { get; set; }
        }
    }
}
