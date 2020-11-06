namespace Bank.Domains.Payment.Services
{
    public class OrderQueryReq : BasePayReq
    {
        // 订单号
        public string OrderNo { get; set; }
        public string FlowNo { get; set; }
        public QueryTypeEnum QueryType { get; set; }
    }
}
