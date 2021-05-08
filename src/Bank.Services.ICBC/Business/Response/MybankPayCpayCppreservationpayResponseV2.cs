namespace Icbc.Business
{
    public class MybankPayCpayCppreservationpayResponseV2 : IcbcResponse
    {
        public string SerialNo { get; set; }

        public string AgreeCode { get; set; }

        public string PartnerSeq { get; set; }

        public string OrderCurr { get; set; }

        public string PayAmount { get; set; }

        public string Status { get; set; }

        public string RedirectParam { get; set; }
    }
}
