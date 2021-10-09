using ExchangeRateFactory.Factory.Dtos;
using ExchangeRateFactory.Factory.Services.Public.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.SimpleDemo.Data
{
    public class ExchangeRateService
    {
        private readonly IExchangeRateService<ExchangeRate, int> _exchangeRateService;
        private readonly Factory.Services.Internal.Interfaces.IExchangeRateService<ExchangeRate, int> _exchangeRateInternalService;
        public ExchangeRateService(
            IExchangeRateService<ExchangeRate, int> exchangeRateService,
            Factory.Services.Internal.Interfaces.IExchangeRateService<ExchangeRate, int> exchangeRateInternalService
            )
        {
            _exchangeRateService = exchangeRateService;
            _exchangeRateInternalService = exchangeRateInternalService;
        }

        public async Task<ExchangeRateSummary<int>[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
        {
            return await _exchangeRateService.GetTodayOrSpecificDate(
                specificDate: specificDate,
                cancellationToken: cancellationToken);
        }

        public async Task<(int, ExchangeRate[])> InsertAndFetch(DateTimeOffset date, CancellationToken cancellationToken = default)
        {
            var newRecords = await _exchangeRateInternalService.ReadAndSaveIfNot(date, cancellationToken);

            var list = await _exchangeRateService.GetTodayOrSpecificDate(
                selector: x => x as ExchangeRate,
                specificDate: date,
                cancellationToken: cancellationToken
                );

            return (newRecords, list);
        }
    }
}
