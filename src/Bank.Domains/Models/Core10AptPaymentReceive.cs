using CPTech.EntityFrameworkCore.Mapping;
using System;
using System.Collections.Generic;

namespace Bank.EFCore.Models
{
    [EntityAttribute("CORE10_Receivable_APT")]

    public partial class Core10AptPaymentReceive
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

        [FieldAttribute("C_PaymentID")]
        public Guid? CPaymentId { get; set; }

        [FieldAttribute("C_RelateID")]
        public Guid? CRelateId { get; set; }

        [FieldAttribute("C_CrossAmout")]
        public decimal? CCrossAmout { get; set; }

        [FieldAttribute("C_CrossMonth")]
        public string CCrossMonth { get; set; }

        [FieldAttribute("C_ReceiveAmountTotal")]
        public decimal? CReceiveAmountTotal { get; set; }

        [FieldAttribute("C_RemainAmount")]
        public decimal? CRemainAmount { get; set; }
    }
}
