namespace Bank.Domains.Payment.Services
{
    public class BasePayRsp
    {
        public int Code { get; set; }
        public string Message { get; set; }
        public PaymentStatus? Status { get; set; }
        public string Data { get; set; }
    }
}
