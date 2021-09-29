using ExchangeRateFactory.Data.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ExchangeRateFactory.Factory.Services.Internal.Interfaces
{
    public interface IExchangeRateLoaderService
    {
        /// <summary>
        /// Verilen tarihe göre ilgili döviz kuru bilgilerini alır ve XmlDocument nesnesi olarak döner
        /// </summary>
        /// <param name="specificDate">
        /// Eğer NULL değerde olursa Now olarak işlem görecektir.
        /// Hangi tarihe ait verilerin getirileceğini belirler.
        /// </param>
        Task<XmlDocument> LoadXml(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verilen tarihe göre ilgili döviz kuru bilgilerini alır ve IExchangeRate nesnesi olarak döner
        /// </summary>
        /// <typeparam name="PK"></typeparam>
        /// <param name="specificDate">
        /// Eğer NULL değerde olursa Now olarak işlem görecektir.
        /// Hangi tarihe ait verilerin getirileceğini belirler.
        /// </param>
        /// <returns></returns>
        Task<T[]> LoadExchangeRate<T, PK>(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
            where T : ExchangeRate<PK>, new()
            where PK : struct;
    }
}
