using ExchangeRateFactory.Common.Extensions;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Factory.Utilities.AuditFile
{
    // TODO: İlgili yerler locklanacak...!
    public static class AuditFileExtensions
    {
        /// <summary>
        /// Eğer audit log dosyası oluşturulması gerekiyorsa oluşturur
        /// </summary>
        /// <param name="settings"></param>
        public static async Task CreateAuditFile(IFactorySettings settings)
        {
            if (settings.AuditIsActive == false)
                return;

            if (File.Exists(settings.AuditFilePath) == false)
                await File.Create(settings.AuditFilePath).DisposeAsync();
        }

        /// <summary>
        /// Audit dosyasına log yazar
        /// </summary>
        /// <param name="exchangeRateDate"></param>
        /// <param name="isSpecificDate">Belirli bir tarih ya da günlük bir işlem olup olmadığını temsil eder</param>
        /// <param name="code">Tanımlayıcı mesaj kodu</param>
        /// <param name="message">Loga yazılacak mesaj içeriği</param>
        /// <returns></returns>
        public static async Task AddAuditLine(this AuditFileModel audit, IFactorySettings settings, CancellationToken cancellationToken = default)
        {
            StringBuilder s = new(audit.ToJson());

            Console.WriteLine($"[ExchangeRateFactory]   {DateTimeOffset.Now.dd_MM_yyyy_HH_mm_ss()}   {s}");

            if (settings.AuditIsActive == false)
                return;

            using StreamWriter tw = new(settings.AuditFilePath, append: true, encoding: Encoding.UTF8);

            await tw.WriteLineAsync(s, cancellationToken);
        }

        /// <summary>
        /// Log dosyasından son
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public static async Task<AuditFileModel> GetLastAudit(IFactorySettings settings, CancellationToken cancellationToken = default)
        {
            if (settings.AuditIsActive == false)
                return null;

            var lastLine = await FileExtensions.ReadLastLineAsync(settings.AuditFilePath, cancellationToken: cancellationToken);

            return JsonExtensions.FromJson<AuditFileModel>(lastLine);
        }
    }
}
