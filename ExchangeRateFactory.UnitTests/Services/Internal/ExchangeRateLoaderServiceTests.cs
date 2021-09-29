using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Services.Internal;
using ExchangeRateFactory.UnitTests.Base;
using ExchangeRateFactory.UnitTests.TestModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xunit;

namespace ExchangeRateFactory.UnitTests.Services
{
    public class ExchangeRateLoaderServiceTests: TestBase
    {
        [Fact]
        public async Task CanReadXmlPage()
        {
            var service = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());

            // https://www.tcmb.gov.tr/kurlar/202005/22052020.xml
            var exchangeRateDate = new DateTimeOffset(2020, 5, 22, 0, 0, 0, TimeSpan.Zero);

            var xmlDocument = await service.LoadXml(exchangeRateDate);

            XmlNodeList node_tarih_dates = xmlDocument.GetElementsByTagName("Tarih_Date");

            Assert.NotNull(node_tarih_dates);
            Assert.Equal(1, node_tarih_dates.Count);

            var node_tarih_date = node_tarih_dates.Item(0);

            Assert.Equal("2020/99", node_tarih_date.Attributes.GetNamedItem("Bulten_No").Value);
            Assert.Equal("05/22/2020", node_tarih_date.Attributes.GetNamedItem("Date").Value);

            var node_currency = node_tarih_date.ChildNodes;
            Assert.Equal(20, node_currency.Count);

            Assert.Equal("Currency", node_currency[0].Name);

            Assert.Equal("USD", node_currency[0].Attributes.GetNamedItem("CurrencyCode").Value);
            Assert.Equal("6.8142", node_currency[0]["BanknoteSelling"].InnerText);

            Assert.Equal("XDR", node_currency[^1].Attributes.GetNamedItem("CurrencyCode").Value);
            Assert.Empty(node_currency[^1]["BanknoteSelling"].InnerText);
            Assert.Equal("1.36226", node_currency[^1]["CrossRateOther"].InnerText);

        }

        [Theory]
        [MemberData(nameof(CanReadExchangeRatesData))]
        public async Task CanReadExchangeRates(ExchangeRateTestModel testModel, int expectedCount)
        {
            var service = new ExchangeRateLoaderService(Logger<ExchangeRateLoaderService>());

            // https://www.tcmb.gov.tr/kurlar/202109/24092021.xml
            var list = (await service.LoadExchangeRate<ExchangeRate<Guid>,Guid>(testModel.ExchangeRateDate))
                .Select(x => ExchangeRateTestModel.MapFrom(x))
                .ToArray();

            Assert.Equal(expectedCount, list.Length);

            Assert.True(list.All(x => x.BulletinNo == testModel.BulletinNo));
            Assert.True(list.All(x => x.ExchangeRateDate.Date == testModel.ExchangeRateDate.Date));
            Assert.True(list.All(x => x.ReleaseDate.Date == testModel.ReleaseDate.Date));

            Assert.Contains(list, x => x.CurrencyCode == "USD");
            Assert.Contains(list, x => x.CurrencyCode == "EUR");

            var a = list.Single(x => x.CurrencyCode == testModel.CurrencyCode);

            Assert.Equal(a, testModel);
        }

        public static IEnumerable<object[]> CanReadExchangeRatesData()
        {
            var p1 = new ExchangeRateTestModel()
            {
                ExchangeRateDate = new DateTimeOffset(2021, 9, 24, 0, 0, 0, TimeSpan.Zero),
                ReleaseDate = new DateTimeOffset(2021, 09, 24, 0, 0 , 0, TimeSpan.Zero),
                BulletinNo = "2021/178",
                Unit = 1,
                CurrencyCode = "USD",
                CurrencyName = "US DOLLAR",
                BanknoteBuying = 8.8178m,
                BanknoteSelling = 8.8531m,
                ForexBuying = 8.8240m,
                ForexSelling = 8.8399m,
            };
            var p2 = new ExchangeRateTestModel()
            {
                ExchangeRateDate = new DateTimeOffset(2021, 8, 24, 0, 0, 0, TimeSpan.Zero),
                ReleaseDate = new DateTimeOffset(2021, 8, 24, 0, 0, 0, TimeSpan.Zero),
                BulletinNo = "2021/156",
                Unit = 1,
                CurrencyCode = "EUR",
                CurrencyName = "EURO",
                BanknoteBuying = 9.8659m,
                BanknoteSelling = 9.9054m,
                ForexBuying = 9.8728m,
                ForexSelling = 9.8906m,
            };
            return new List<object[]>()
            {
                new object[]{ p1, 21 },
                new object[]{ p2, 21 },
            };
        }
    }
}
