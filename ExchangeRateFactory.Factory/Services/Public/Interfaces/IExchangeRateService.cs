using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Factory.Services.Public.Interfaces
{
    public interface IExchangeRateService<T, PK>
        where T : Data.Entities.ExchangeRate<PK>
        where PK : struct
    {
        Task<Dtos.ExchangeRateSummary<PK>[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, string[] currencyCodes = null, CancellationToken cancellationToken = default);
    }
}
