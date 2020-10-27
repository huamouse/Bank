using CPTech.EntityFrameworkCore.Mapping;
using System;
using System.Collections.Generic;

namespace Bank.EFCore.Models
{
    [Entity("Core10_Payment_Apt")]
    public partial class Core10PaymentApt
    {
        public Guid Id { get; set; }
        public bool Deleted { get; set; }

        [FieldAttribute("CREATED_BY")]
        public Guid? CreatedBy { get; set; }

        [FieldAttribute("DATE_ENTERED")]
        public DateTime DateEntered { get; set; }

        [FieldAttribute("MODIFIED_USER_ID")]
        public Guid? ModifiedUserId { get; set; }

        [FieldAttribute("DATE_MODIFIED")]
        public DateTime DateModified { get; set; }

        [FieldAttribute("C_ContractID")]
        public Guid? CContractId { get; set; }

        [FieldAttribute("c_paymenttype")]
        public string CPaymentType { get; set; }

        [FieldAttribute("C_PaymentTotal")]
        public decimal? CPaymentTotal { get; set; }

        [FieldAttribute("C_PaymentDate")]
        public DateTime? CPaymentDate { get; set; }

        [FieldAttribute("C_ReceiveUnit")]
        public string CReceiveUnit { get; set; }

        [FieldAttribute("C_IsRegist")]
        public string CIsRegist { get; set; }

        [FieldAttribute("C_NOtes")]
        public string CNotes { get; set; }

        [FieldAttribute("C_Status")]
        public string CStatus { get; set; }

        [FieldAttribute("C_IsInvoice")]
        public int? CIsInvoice { get; set; }

        [FieldAttribute("C_ispush")]
        public string CIspush { get; set; }

        [FieldAttribute("strC_hongchong")]
        public string StrCHongchong { get; set; }
    }
}
