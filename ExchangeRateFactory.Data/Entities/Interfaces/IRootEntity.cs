using System;

namespace ExchangeRateFactory.Data.Entities.Interfaces
{
    public interface IRootEntity<PK> where PK: struct
    {
        PK Id { get; set; }

        public DateTimeOffset CreateDate { get; set; }
        public DateTimeOffset UpdateDate { get; set; }
    }
}
