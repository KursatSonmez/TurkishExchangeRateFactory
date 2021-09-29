﻿using ExchangeRateFactory.Data.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ExchangeRateFactory.Data.Mapping
{
    public class ExchangeRateMap<PK> : IEntityTypeConfiguration<ExchangeRate<PK>> where PK : struct
    {
        public void Configure(EntityTypeBuilder<ExchangeRate<PK>> builder)
        {
            builder.ToTable("ExchangeRates");

            builder.Property(x => x.ForexBuying).HasPrecision(18, 5);
            builder.Property(x => x.ForexSelling).HasPrecision(18, 5);

            builder.Property(x => x.BanknoteBuying).HasPrecision(18, 5);
            builder.Property(x => x.BanknoteSelling).HasPrecision(18, 5);

            builder.Property(x => x.CrossRateUSD).HasPrecision(18, 5);
            builder.Property(x => x.CrossRateOther).HasPrecision(18, 5);

            builder.HasIndex(x => new { x.ExchangeRateDate, x.CurrencyCode }).IsUnique();
        }
    }
}
