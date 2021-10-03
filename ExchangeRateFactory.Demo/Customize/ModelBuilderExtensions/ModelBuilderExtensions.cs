using ExchangeRateFactory.Data.ModelBuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateFactory.Demo.Customize.ModelBuilderExtensions
{
    public static class ModelBuilderExtensions
    {
        public static void UseExchangeRates(this ModelBuilder modelBuilder)
            => modelBuilder.UseExchangeRateFactory<System.Guid>();
    }
}
