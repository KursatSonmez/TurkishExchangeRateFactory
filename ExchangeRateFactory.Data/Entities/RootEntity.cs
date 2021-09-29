using ExchangeRateFactory.Data.Entities.Interfaces;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExchangeRateFactory.Data.Entities
{
    public abstract class RootEntity<PK> : IRootEntity<PK> where PK : struct
    {
        [Key]
        public PK Id { get; set; }

        [Required]
        public DateTimeOffset CreateDate { get; set; }

        [Required]
        public DateTimeOffset UpdateDate { get; set; }
    }
}
