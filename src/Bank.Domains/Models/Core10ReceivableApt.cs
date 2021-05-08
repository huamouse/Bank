using System;
using CPTech.EFCore.Mapping;

namespace Bank.EFCore.Models
{
    [Entity("CORE10_Receivable_APT")]
    public partial class Core10ReceivableApt
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

        [Field("C_ReceiveType")]
        public string CReceiveType { get; set; }

        [Field("C_StartTime")]
        public DateTime? CStartTime { get; set; }

        [Field("C_EndTime")]
        public DateTime? CEndTime { get; set; }

        [Field("C_ReceivableMoney")]
        public decimal? CReceivableMoney { get; set; }

        [Field("C_ActualMoney")]
        public decimal? CActualMoney { get; set; }

        [Field("C_Flag")]
        public int? CFlag { get; set; }

        [Field("C_TheoreticalMoney")]
        public decimal? CTheoreticalMoney { get; set; }
    }
}
