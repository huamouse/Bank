using CPTech.EntityFrameworkCore.Mapping;
using System;
using System.Collections.Generic;

namespace Bank.EFCore.Models
{
    [EntityAttribute("CORE10_Receivable_APT")]
    public partial class Core10ReceivableApt
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

        [FieldAttribute("C_ReceiveType")]
        public string CReceiveType { get; set; }

        [FieldAttribute("C_StartTime")]
        public DateTime? CStartTime { get; set; }

        [FieldAttribute("C_EndTime")]
        public DateTime? CEndTime { get; set; }

        [FieldAttribute("C_ReceivableMoney")]
        public decimal? CReceivableMoney { get; set; }

        [FieldAttribute("C_ActualMoney")]
        public decimal? CActualMoney { get; set; }

        [FieldAttribute("C_Flag")]
        public int? CFlag { get; set; }

        [FieldAttribute("C_TheoreticalMoney")]
        public decimal? CTheoreticalMoney { get; set; }
    }
}
