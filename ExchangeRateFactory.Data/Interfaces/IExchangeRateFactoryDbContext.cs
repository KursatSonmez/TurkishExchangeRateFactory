using ExchangeRateFactory.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExchangeRateFactory.Data.Interfaces
{
    public interface IExchangeRateFactoryDbContext<T, PK> : IDisposable
        where T : ExchangeRate<PK>
        where PK : struct
    {
        DbSet<T> ExchangeRates { get; set; }
    }
}
