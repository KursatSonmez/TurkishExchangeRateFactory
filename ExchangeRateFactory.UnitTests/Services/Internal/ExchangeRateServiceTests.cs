using ExchangeRateFactory.Demo.Customize.Entities;
using ExchangeRateFactory.Factory.Services.Internal;
using ExchangeRateFactory.Factory.Services.Internal.Interfaces;
using ExchangeRateFactory.UnitTests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeRateFactory.UnitTests.Services.Internal
{
    public class ExchangeRateServiceTests : TestBase
    {
        [Fact]
        public async Task TestChangeSelectExpression()
        {
            var expressions = DefaultExpressions;
            string databaseName = GetDatabaseName();
            IExchangeRateLoaderService loaderService = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());
            var dbContext = NewDbContext(databaseName);
            var service = new ExchangeRateService<ExchangeRate, Guid>(loaderService, dbContext, expressions);

            // https://www.tcmb.gov.tr/kurlar/202109/24092021.xml
            var exchangeRateDate = new DateTimeOffset(2021, 9, 24, 0, 0, 0, TimeSpan.Zero);
            var count = await service.ReadAndSaveIfNot(exchangeRateDate);

            Assert.Equal(21, count);

            Assert.True(
                await service.AnyAsync(exchangeRateDate)
                );

            await dbContext.DisposeAsync();
            loaderService = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());
            dbContext = NewDbContext(databaseName);
            service = new ExchangeRateService<ExchangeRate, Guid>(loaderService, dbContext, expressions);

            expressions.GetSelectExpression = (date) =>
            x => x.ExchangeRateDate.Date == date.Date && x.CurrencyCode == "CCC";

            Assert.False(
                await service.AnyAsync(exchangeRateDate)
                );

            expressions.GetSelectExpression = (date) =>
            x => x.ExchangeRateDate.Date == date.Date && x.CurrencyCode == "USD";

            Assert.True(
                await service.AnyAsync(exchangeRateDate)
                );


            expressions.GetSelectExpression = (date) =>
            x => x.ExchangeRateDate.Date == date.Date;

            Assert.True(
                await service.AnyAsync(exchangeRateDate)
                );
        }


        [Fact]
        public async Task CanReadAndSave()
        {
            string databaseName = GetDatabaseName();
            IExchangeRateLoaderService loaderService = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());
            var dbContext = NewDbContext(databaseName);
            var service = new ExchangeRateService<ExchangeRate, Guid>(
                loaderService,
                dbContext,
                DefaultExpressions
                );

            // https://www.tcmb.gov.tr/kurlar/202109/24092021.xml
            var exchangeRateDate = new DateTimeOffset(2021, 9, 24, 0, 0, 0, TimeSpan.Zero);
            var count = await service.ReadAndSaveIfNot(exchangeRateDate);

            Assert.Equal(21, count);

            Assert.True(
                await service.AnyAsync(exchangeRateDate)
                );

            var usd = dbContext.ExchangeRates.Single(x => x.CurrencyCode == "USD" && x.ExchangeRateDate.Date == exchangeRateDate.Date);
            Assert.Equal(8.8531m, usd.BanknoteSelling);

            await dbContext.DisposeAsync();

            dbContext = NewDbContext(databaseName);
            dbContext.ExchangeRates.RemoveRange(dbContext.ExchangeRates.ToList());
            dbContext.SaveChanges();

            await dbContext.DisposeAsync();

            dbContext = NewDbContext(databaseName);

            loaderService = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());
            service = new ExchangeRateService<ExchangeRate, Guid>(loaderService, dbContext, DefaultExpressions);

            Assert.False(
                await service.AnyAsync(exchangeRateDate)
                );
        }
    }
}
