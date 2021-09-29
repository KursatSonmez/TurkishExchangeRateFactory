using ExchangeRateFactory.Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using Xunit;

namespace ExchangeRateFactory.UnitTests.Units
{
    public class JsonExtensionTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("{\"Name\":\"serialize name\",\"No\":123,\"NoNullable\":123.4,\"Date\":\"2021-09-26T00:00:00+00:00\",\"DateNullable\":\"2021-09-26T00:00:00+00:00\",\"AEnum\":\"Fail\",\"AEnumNullable\":\"Success\"}")]
        public void CanDeserialize(string value)
        {
            var obj = JsonExtensions.FromJson<AClass>(value);

            if (string.IsNullOrWhiteSpace(value))
                Assert.Null(obj);
            else
                Assert.NotNull(obj);
        }

        [Fact]
        public void CanSerialize()
        {
            AClass a = new()
            {
                AEnum = AEnum.Fail,
                AEnumNullable = AEnum.Success,
                Date = new DateTimeOffset(2021, 09, 26, 0, 0, 0, TimeSpan.Zero),
                DateNullable = new DateTimeOffset(2021, 09, 26, 0, 0, 0, TimeSpan.Zero),
                Name = "serialize name",
                No = 123,
                NoNullable = 123.4m,
            };

            var str = JsonExtensions.ToJson(a);

            Assert.NotNull(str);

            Assert.Equal(
                "{\"Name\":\"serialize name\",\"No\":123,\"NoNullable\":123.4,\"Date\":\"2021-09-26T00:00:00+00:00\",\"DateNullable\":\"2021-09-26T00:00:00+00:00\",\"AEnum\":\"Fail\",\"AEnumNullable\":\"Success\"}",
                str
                );
        }
    }

    public class AClass
    {
        public string Name { get; set; }
        public int No { get; set; }
        public decimal? NoNullable { get; set; }
        public DateTimeOffset Date { get; set; }
        public DateTimeOffset? DateNullable { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AEnum AEnum { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public AEnum? AEnumNullable { get; set; }
    }
    public enum AEnum
    {
        Success,
        Fail
    }
}
