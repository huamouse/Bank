using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class MybankPayCpayCppayapplyResponseV2 : IcbcResponse
    {
        [JsonPropertyName("serialNo")]
        public string SerialNo { get; set; }
        [JsonPropertyName("agreeCode")]
        public string AgreeCode { get; set; }
        [JsonPropertyName("agreeName")]
        public string AgreeName { get; set; }
        [JsonPropertyName("partnerSeq")]
        public string PartnerSeq { get; set; }
        [JsonPropertyName("payMode")]
        public string PayMode { get; set; }
        [JsonPropertyName("orderAmount")]
        public string OrderAmount { get; set; }
        [JsonPropertyName("orderCurr")]
        public string OrderCurr { get; set; }
        [JsonPropertyName("sumPayamt")]
        public string SumPayamt { get; set; }
        [JsonPropertyName("status")]
        public string Status { get; set; }
        [JsonPropertyName("redirectParam")]
        public string RedirectParam { get; set; }
    }
}
