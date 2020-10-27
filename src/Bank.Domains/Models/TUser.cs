using System;
using System.Collections.Generic;

namespace Bank.EFCore.Models
{
    public partial class TUser
    {
        public string UserId { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public decimal Id { get; set; }
        public decimal? Grade { get; set; }
        public DateTime? LastLogin { get; set; }
        public decimal? Logins { get; set; }
        public DateTime? ChgPwdTime { get; set; }
        public decimal? ChgPwdLimit { get; set; }
        public decimal? Status { get; set; }
        public string Iplimit { get; set; }
        public string CertNo { get; set; }
        public decimal? OrgId { get; set; }
        public byte[] Photo { get; set; }
        public decimal? Zxjl { get; set; }
        public string Phone { get; set; }
    }
}
