using ExchangeRateFactory.Demo.Data.Entities;
using ExchangeRateFactory.Factory.Services.Public;
using ExchangeRateFactory.UnitTests.Base;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace ExchangeRateFactory.UnitTests.Services.Public
{
    public class ExchangeRateServiceTests : TestBase
    {
        [Fact]
        public async Task TestGetTodayOrSpecificDate()
        {
            string databaseName = GetDatabaseName();
            // önce belirli bir tarihe ait veriler kaydedilir
            Factory.Services.Internal.Interfaces.IExchangeRateLoaderService loaderService = new Factory.Services.Internal.ExchangeRateLoaderService(Logger<Factory.Services.Internal.ExchangeRateLoaderService>());
            var internalService = new ExchangeRateFactory.Factory.Services.Internal.ExchangeRateService<ExchangeRate, Guid>(
                loaderService,
                NewDbContext(databaseName),
                DefaultExpressions
                );

            // https://www.tcmb.gov.tr/kurlar/202107/13072021.xml
            var exchangeRateDate = new DateTimeOffset(2021, 7, 13, 0, 0, 0, TimeSpan.Zero);
            await internalService.ReadAndSaveIfNot(exchangeRateDate);

            // Public servis oluşturulur
            var dbContext = NewDbContext(databaseName);
            var service = new ExchangeRateService<ExchangeRate, Guid>(
                dbContext,
                DefaultExpressions
                );

            // tümü getirilir
            var list = await service.GetTodayOrSpecificDate(exchangeRateDate);

            Assert.Equal(20, list.Length);

            var usd = list.SingleOrDefault(x => x.CurrencyCode == "USD");
            Assert.NotNull(usd);
            Assert.Equal(8.6318m, usd.BanknoteSelling);

            var eur = list.SingleOrDefault(x => x.CurrencyCode == "EUR");
            Assert.NotNull(eur);
            Assert.Equal(10.2263m, eur.BanknoteSelling);

            // sadece 3 para birimi getirilir
            var currencyCodes = new[] { "CHF", "EUR", "USD" };
            list = await service.GetTodayOrSpecificDate(exchangeRateDate, currencyCodes);

            Assert.Equal(3, list.Length);
            Assert.True(
                list.Select(x => x.CurrencyCode).SequenceEqual(currencyCodes)
                );

            var usd2 = list.SingleOrDefault(x => x.CurrencyCode == "USD");
            Assert.NotNull(usd2);
            Assert.Equal(usd.Id, usd2.Id);

            var chf = list.SingleOrDefault(x => x.CurrencyCode == "CHF");
            Assert.NotNull(chf);
            Assert.Equal(9.4460m, chf.BanknoteSelling);
        }

    }
}
