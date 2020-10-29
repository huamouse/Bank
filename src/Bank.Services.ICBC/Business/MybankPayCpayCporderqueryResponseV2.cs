using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Icbc.Business
{
    public class MybankPayCpayCporderqueryResponseV2 : IcbcResponse
    {
        public string SerialNo { get; set; }

        public string AgreeCode { get; set; }

        public string AgreeName { get; set; }

        public string PartnerSeq { get; set; }

        public string PayMode { get; set; }

        public string PayType { get; set; }

        public string OrderAmount { get; set; }

        public string OrderCurr { get; set; }
        [JsonPropertyName("sumPayamt")]
        public string SumPayAmt { get; set; }

        public string Source { get; set; }

        public string AccrualCny { get; set; }

        public string AccrualForeign { get; set; }
        [JsonPropertyName("payerAccno")]
        public string PayerAccNo { get; set; }
        [JsonPropertyName("payerAccname")]
        public string PayerAccName { get; set; }
        [JsonPropertyName("payerAccnoForeign")]
        public string PayerAccNoForeign { get; set; }
        [JsonPropertyName("payerAccnameForeign")]
        public string PayerAccnameForeign { get; set; }

        public string PayStatus { get; set; }

        public string EnterpriseName { get; set; }

        public string PayRemark { get; set; }

        public string TxCode { get; set; }

        public string TxRem { get; set; }

        public string PartnerSeqOrigin { get; set; }

        public string PayerSysFlag { get; set; }

        public string PayChannel { get; set; }

        public string TppmBankCode { get; set; }

        public List<PayPlan> PayPlanList { get; set; }

        public List<Payee> PayeeList { get; set; }

        public List<Goods> GoodsList { get; set; }

        public List<Fee> FeeList { get; set; }
        [JsonPropertyName("czCardInfoList")]
        public List<CardInfo> CardInfoList { get; set; }

        public class PayPlan
        {
            public string PayPlanSubcode { get; set; }

            public string PayEntity { get; set; }

            public string PayAmount { get; set; }

            public string PayCurr { get; set; }

            public string Status { get; set; }

            public string AppendFlag { get; set; }
            [JsonPropertyName("errno")]
            public string ErrNo { get; set; }
            [JsonPropertyName("errmsg")]
            public string ErrMsg { get; set; }
            [JsonPropertyName("billno")]
            public string BillNo { get; set; }
            [JsonPropertyName("payeeAccno")]
            public string PayeeAccNo { get; set; }
        }

        public class Payee
        {
            public string PayAmount { get; set; }

            public string PayeeCompanyName { get; set; }
            [JsonPropertyName("payeeAccno")]
            public string PayeeAccNo { get; set; }

            public string PayeeBankCountry { get; set; }

            public string PayeeBankSign { get; set; }
            [JsonPropertyName("rbankname")]
            public string BankName { get; set; }

            public string PayeeCountry { get; set; }

            public string RacAddress1 { get; set; }

            public string RacAddress2 { get; set; }

            public string RacAddress3 { get; set; }

            public string RacAddress4 { get; set; }

            public string RacPostcode { get; set; }

            public string Agentbic { get; set; }

            public string PayeeAddress { get; set; }

            public string PayeeOrgcode { get; set; }

            public string MallName { get; set; }
        }

        public class Goods
        {
            public string PayeeCompanyName { get; set; }

            public string GoodsName { get; set; }

            public string GoodsNumber { get; set; }

            public string GoodsAmt { get; set; }
        }

        public class Fee
        {
            public string ExpenseType { get; set; }

            public string PayerFeeCurr { get; set; }

            public string Expense { get; set; }

            public string FeeFlag { get; set; }

            public string ChargeFlag { get; set; }
        }

        public class CardInfo
        {
            [JsonPropertyName("czCard")]
            public string Card { get; set; }
            [JsonPropertyName("czCardName")]
            public string CardName { get; set; }

            public string ActualAmount { get; set; }
        }
    }
}
