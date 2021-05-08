namespace Icbc.Business
{
    public class MybankPayCpayCpordercloseResponseV1 : IcbcResponse
    {
        public string ErrorNo { get; set; }

        public string Status { get; set; }

        public string AgreeCode { get; set; }

        public string PartnerSeq { get; set; }
    }
}
