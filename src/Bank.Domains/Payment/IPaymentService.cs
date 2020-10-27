using System;
using System.Collections.Generic;
using System.Text;

namespace Bank.Domains.Payment
{
    public interface IPaymentService
    {
        string Pay(Payment payment);
        string Query(PayQuery payQuery);
        string Notify(Payment payment);
    }

    public class BasePay
    {
        // 订单号
        public string OrderNo { get; set; }
        // 支付类型
        public PayTypeEnum PayType { get; set; }
        // 支付通道
        public ChannelEnum Channel { get; set; }
    }

    public class Payment : BasePay
    {
        // 支付事由
        public string Note { get; set; }
        // 支付金额，单位：分
        public long Amount { get; set; }
        // 付款方
        public string Payer { get; set; }
        // 收款方
        public string Payee { get; set; }
        // 通知url
        public string NotifyUrl { get; set; }
    }

    public class PayQuery : BasePay
    {
        public string FlowNo { get; set; }
        public QueryTypeEnum QueryType { get; set; }
    }
}
