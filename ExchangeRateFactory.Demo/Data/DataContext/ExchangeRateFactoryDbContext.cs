using ExchangeRateFactory.Data.ModelBuilderExtensions;
using ExchangeRateFactory.Demo.Data.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace ExchangeRateFactory.Demo.Data.DataContext
{
    public class ExchangeRateFactoryDbContext : DbContext, ExchangeRateFactory.Data.Interfaces.IExchangeRateFactoryDbContext<ExchangeRate, Guid>
    {
        public ExchangeRateFactoryDbContext() : base()
        {
        }

        private readonly DbContextOptions<ExchangeRateFactoryDbContext> _options;
        public ExchangeRateFactoryDbContext(DbContextOptions<ExchangeRateFactoryDbContext> options) : base(options)
        {
            _options = options;
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public static ExchangeRateFactoryDbContext Create() => new();
        public static ExchangeRateFactoryDbContext Create(DbContextOptions<ExchangeRateFactoryDbContext> options)
             => new(options);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseExchangeRateFactory<Guid>();
        }

    }
}
