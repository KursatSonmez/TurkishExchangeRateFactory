using ExchangeRateFactory.Demo.Customize.Entities;
using System;

namespace ExchangeRateFactory.Demo.Customize.Interfaces
{
    public interface IExchangeRateFactoryDbContext : ExchangeRateFactory.Data.Interfaces.IExchangeRateFactoryDbContext<ExchangeRate, Guid>
    {
    }
}
