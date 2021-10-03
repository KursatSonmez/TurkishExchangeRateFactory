using ExchangeRateFactory.Demo.Customize.Entities;
using ExchangeRateFactory.Demo.Customize.Interfaces;
using ExchangeRateFactory.Demo.Customize.ModelBuilderExtensions;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateFactory.Demo.Customize.DataContext
{
    public class ExchangeRateFactoryDbContext : DbContext, IExchangeRateFactoryDbContext
    {
        public ExchangeRateFactoryDbContext() : base()
        {
        }

        public ExchangeRateFactoryDbContext(DbContextOptions<ExchangeRateFactoryDbContext> options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public static ExchangeRateFactoryDbContext Create() => new();
        public static ExchangeRateFactoryDbContext Create(DbContextOptions<ExchangeRateFactoryDbContext> options)
             => new(options);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseExchangeRates();
        }

    }
}
