using ExchangeRateFactory.Data.Mapping;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateFactory.Data.ModelBuilderExtensions
{
    public static class ModelBuilderExtensions
    {
        public static void UseExchangeRateFactory<PK>(this ModelBuilder modelBuilder) where PK : struct
        {
            var map = new ExchangeRateMap<PK>();

            modelBuilder.ApplyConfiguration(map);
        }
    }
}
