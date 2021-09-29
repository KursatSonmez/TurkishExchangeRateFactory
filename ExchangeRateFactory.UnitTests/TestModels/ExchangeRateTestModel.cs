using ExchangeRateFactory.Data.Entities;
using System;

namespace ExchangeRateFactory.UnitTests.TestModels
{
    public class ExchangeRateTestModel
    {
        public DateTimeOffset ExchangeRateDate { get; set; }
        public DateTimeOffset ReleaseDate { get; set; }
        public string BulletinNo { get; set; }
        public string CurrencyCode { get; set; }
        public int Unit { get; set; }
        public string CurrencyName { get; set; }
        public decimal ForexBuying { get; set; }
        public decimal ForexSelling { get; set; }
        public decimal BanknoteBuying { get; set; }
        public decimal BanknoteSelling { get; set; }

        public static ExchangeRateTestModel MapFrom(ExchangeRate<Guid> exchangeRate)
            => new()
            {
                BanknoteBuying = exchangeRate.BanknoteBuying,
                BanknoteSelling = exchangeRate.BanknoteSelling,
                BulletinNo = exchangeRate.BulletinNo,
                CurrencyCode = exchangeRate.CurrencyCode,
                CurrencyName = exchangeRate.CurrencyName,
                ExchangeRateDate = exchangeRate.ExchangeRateDate,
                ForexBuying = exchangeRate.ForexBuying,
                ForexSelling = exchangeRate.ForexSelling,
                ReleaseDate = exchangeRate.ReleaseDate,
                Unit = exchangeRate.Unit,
            };

        public override int GetHashCode()
            => CurrencyCode.GetHashCode();

        public override bool Equals(object obj)
            => this.Equals(obj as ExchangeRateTestModel);

        public static bool operator ==(ExchangeRateTestModel leftObj, ExchangeRateTestModel rightObj)
        {
            if (leftObj is null)
            {
                if (rightObj is null)
                    return true;

                return false;
            }

            return leftObj.Equals(rightObj);
        }
        public static bool operator !=(ExchangeRateTestModel leftObj, ExchangeRateTestModel rightObj)
            => !(leftObj == rightObj);

        private bool Equals(ExchangeRateTestModel obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (this.GetType() != obj.GetType())
                return false;

            return this.ExchangeRateDate.Date == obj.ExchangeRateDate.Date
                && this.ReleaseDate.Date == obj.ReleaseDate.Date
                && this.BulletinNo == obj.BulletinNo
                && this.CurrencyCode == obj.CurrencyCode
                && this.Unit == obj.Unit
                && this.CurrencyName == obj.CurrencyName
                && this.ForexBuying == obj.ForexBuying
                && this.ForexSelling == obj.ForexSelling
                && this.BanknoteBuying == obj.BanknoteBuying
                && this.BanknoteSelling == obj.BanknoteSelling;
        }
    }
}
