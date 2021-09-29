using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Factory.Services.Internal.Interfaces
{
    public interface IExchangeRateService<T, PK>
        where T : Data.Entities.ExchangeRate<PK>
        where PK : struct
    {
        /// <summary>
        /// TCMB Web XML sayfasını okur ve aldığı verileri veri tabanına kaydeder.
        /// 
        /// <strong>Eğer ilgili tarihe ait veri varsa herhangi bir işlem yapmaz</strong>
        /// </summary>
        /// <param name="exchangeRateDate">Hangi tarihe ait verilerin getirileceğini belirleyen parametredir</param>
        /// <param name="cancellationToken"></param>
        /// <returns>Kaydedilen veri sayısı</returns>
        Task<int> ReadAndSaveIfNot(DateTimeOffset exchangeRateDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Tarihe ait kur bilgilerinin olup olmadığını kontrol eder
        /// </summary>
        /// <param name="exchangeRateDate">Hangi tarihe ait verilerin kontrol edileceğini belirleyen parametredir</param>
        /// <param name="cancellationToken"></param>
        /// <returns>
        /// Eğer bu tarihe ait veri varsa true, yoksa false döner.
        /// </returns>
        Task<bool> AnyAsync(DateTimeOffset exchangeRateDate, CancellationToken cancellationToken = default);
    }
}
