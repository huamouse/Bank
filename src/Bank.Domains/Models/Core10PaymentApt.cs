using System;
using CPTech.EFCore.Mapping;

namespace Bank.EFCore.Models
{
    [Entity("Core10_Payment_Apt")]
    public partial class Core10PaymentApt
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }

        [Field("CREATED_BY")]
        public Guid? CreatedBy { get; set; }

        [Field("DATE_ENTERED")]
        public DateTime DateEntered { get; set; }

        [Field("MODIFIED_USER_ID")]
        public Guid? ModifiedUserId { get; set; }

        [Field("DATE_MODIFIED")]
        public DateTime DateModified { get; set; }

        [Field("C_ContractID")]
        public Guid? CContractId { get; set; }

        [Field("c_paymenttype")]
        public string CPaymentType { get; set; }

        [Field("C_PaymentTotal")]
        public decimal? CPaymentTotal { get; set; }

        [Field("C_PaymentDate")]
        public DateTime? CPaymentDate { get; set; }

        [Field("C_ReceiveUnit")]
        public string CReceiveUnit { get; set; }

        [Field("C_IsRegist")]
        public string CIsRegist { get; set; }

        [Field("C_NOtes")]
        public string CNotes { get; set; }

        [Field("C_Status")]
        public string CStatus { get; set; }

        [Field("C_IsInvoice")]
        public int? CIsInvoice { get; set; }

        [Field("C_ispush")]
        public string CIspush { get; set; }

        [Field("strC_hongchong")]
        public string StrCHongchong { get; set; }
    }
}
