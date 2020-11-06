using System;
using System.ComponentModel.DataAnnotations.Schema;
using CPTech.Core;

namespace Bank.Domains.Enities
{
    public class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public long Id { get; set; }

        public long? CreatorId { get; set; }

        public DateTime? CreationTime { get; set; }

        public bool IsDeleted { get; set; }

        public BaseEntity()
        {
            Id = SnowFlake.NextId();
        }
    }
}
