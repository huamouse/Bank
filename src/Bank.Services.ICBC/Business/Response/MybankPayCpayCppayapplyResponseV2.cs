using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class MybankPayCpayCppayapplyResponseV2 : IcbcResponse
    {
        public string SerialNo { get; set; }

        public string AgreeCode { get; set; }

        public string AgreeName { get; set; }

        public string PartnerSeq { get; set; }

        public object PayMode { get; set; }

        public string OrderAmount { get; set; }

        public string OrderCurr { get; set; }
        [JsonPropertyName("sumPayamt")]
        public string SumPayAmt { get; set; }
        public int? Status { get; set; }

        public string RedirectParam { get; set; }
    }
}
