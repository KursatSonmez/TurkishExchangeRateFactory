using ExchangeRateFactory.Demo.Customize.Dtos;
using ExchangeRateFactory.Demo.Customize.Entities;
using ExchangeRateFactory.Factory.Services.Public.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Demo.Customize.Services
{
    public interface IExchangeRateService
    {
        Task<ExchangeRateComboBoxDto[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default);
    }
    public class ExchangeRateService : IExchangeRateService
    {
        private readonly IExchangeRateService<ExchangeRate, Guid> _exchangeRateService;

        public ExchangeRateService(
            IExchangeRateService<ExchangeRate, Guid> exchangeService
            )
        {
            _exchangeRateService = exchangeService;
        }

        public async Task<ExchangeRateComboBoxDto[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
        {
            var codes = new[] { "USD", "EUR", "CHF" };

            return (
                await _exchangeRateService.GetTodayOrSpecificDate(x =>
                new ExchangeRateComboBoxDto
                {
                    BanknoteSelling = x.BanknoteSelling,
                    CurrencyCode = x.CurrencyCode,
                    Id = x.Id,
                },
                specificDate,
                codes,
                cancellationToken)
                )
                .ToArray();
        }
    }
}
