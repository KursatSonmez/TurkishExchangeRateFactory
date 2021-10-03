using ExchangeRateFactory.Factory.Dtos;
using System;

namespace ExchangeRateFactory.Demo.Customize.Dtos
{
    public class ExchangeRateComboBoxDto : ExchangeRateSummary<Guid>
    {
        public static ExchangeRateComboBoxDto MapFrom(ExchangeRateSummary<Guid> dto) =>
            new()
            {
                BanknoteSelling = dto.BanknoteSelling,
                CurrencyCode = dto.CurrencyCode,
                Id = dto.Id,
            };
    }
}
