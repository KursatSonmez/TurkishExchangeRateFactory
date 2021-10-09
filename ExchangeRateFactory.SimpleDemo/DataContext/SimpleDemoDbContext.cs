using ExchangeRateFactory.Data.Interfaces;
using ExchangeRateFactory.Data.ModelBuilderExtensions;
using ExchangeRateFactory.SimpleDemo.Data;
using Microsoft.EntityFrameworkCore;

namespace ExchangeRateFactory.SimpleDemo.DataContext
{
    public class SimpleDemoDbContext : DbContext, IExchangeRateFactoryDbContext<ExchangeRate, int>
    {
        public SimpleDemoDbContext(DbContextOptions<SimpleDemoDbContext> options) : base(options)
        {
        }

        public DbSet<ExchangeRate> ExchangeRates { get; set; }

        public static SimpleDemoDbContext Create(DbContextOptions<SimpleDemoDbContext> options)
             => new(options);

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.UseExchangeRateFactory<int>();
        }
    }
}
