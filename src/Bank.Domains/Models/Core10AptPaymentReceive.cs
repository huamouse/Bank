using System;
using CPTech.EFCore.Mapping;

namespace Bank.EFCore.Models
{
    [Entity("CORE10_Receivable_APT")]

    public partial class Core10AptPaymentReceive
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

        [Field("C_PaymentID")]
        public Guid? CPaymentId { get; set; }

        [Field("C_RelateID")]
        public Guid? CRelateId { get; set; }

        [Field("C_CrossAmout")]
        public decimal? CCrossAmout { get; set; }

        [Field("C_CrossMonth")]
        public string CCrossMonth { get; set; }

        [Field("C_ReceiveAmountTotal")]
        public decimal? CReceiveAmountTotal { get; set; }

        [Field("C_RemainAmount")]
        public decimal? CRemainAmount { get; set; }
    }
}
