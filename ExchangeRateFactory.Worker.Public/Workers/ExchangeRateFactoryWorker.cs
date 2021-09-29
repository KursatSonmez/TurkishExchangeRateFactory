using ExchangeRateFactory.Common.Extensions;
using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Utilities.AuditFile;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Worker.Public.Workers
{
    public class ExchangeRateFactoryWorker<T, PK>
        where T : ExchangeRate<PK>
        where PK : struct
    {
        protected readonly Factory.Services.Internal.Interfaces.IExchangeRateService<T, PK> _internalExchangeRateService;
        protected readonly ILogger<ExchangeRateFactoryWorker<T, PK>> _logger;
        protected readonly IFactorySettings _settings;
        public ExchangeRateFactoryWorker(
            ILogger<ExchangeRateFactoryWorker<T, PK>> logger,
            IFactorySettings factorySettings,
            Factory.Services.Internal.Interfaces.IExchangeRateService<T, PK> internalExchangeRateService
            )
        {
            _logger = logger;
            _settings = factorySettings;
            _internalExchangeRateService = internalExchangeRateService;
        }

        protected AuditFileModel LastAction = null;

        public virtual async Task Execute(CancellationToken cancellationToken = default)
        {
            await this.SetLastActionFromAuditFile(cancellationToken);

            var now = DateTimeOffset.Now;

            var hh = now.ToString("HH");
            // Eğer çalışma saati değilse hiçbir işlem yapılmaz
            if (SkipWorkingHour == false && _settings.WorkingHour != hh)
            {
                Console.WriteLine($"[ExchangeRateFactory]   {DateTimeOffset.Now.dd_MM_yyyy_HH_mm_ss()}   [WorkingHour] Transfer will not start (CurrentHour | WorkingHour = {hh} | {_settings.WorkingHour})");
                if (LastAction?.AuditStatus == AuditStatus.Error)
                {
                    _logger.LogError($"Son döviz alımı hatalı! Audit = {LastAction.ToJson()}");
                }
                return;
            }

            // eğer son işlem bu güne ait ise ve başarılıysa,
            // bugünün döviz verileri zaten alınmış demektir. Herhangi bir işlem yapılmaz
            if (LastAction?.ExchangeRateDate.HasValue == true
                && now.Date == LastAction.ExchangeRateDate.Value.Date
                && LastAction.AuditStatus == AuditStatus.Success)
            {
                Console.WriteLine($"[ExchangeRateFactory]   {DateTimeOffset.Now.dd_MM_yyyy_HH_mm_ss()}   [LastAction] Transfer will not start (CurrentDate | LastActionDate = {now.dd_MM_yyyy()} | {LastAction.ExchangeRateDate.dd_MM_yyyy()})");
                return;
            }

            string statusText = LastAction != null && LastAction.AuditStatus != AuditStatus.Success
                ? $"({LastAction.AuditStatus.ToString().ToUpper()})"
                : null;
            Console.WriteLine($"[ExchangeRateFactory]   {DateTimeOffset.Now.dd_MM_yyyy_HH_mm_ss()}   [Begin] Transfer begins! (CurrentDate | LastActionDate = {now.dd_MM_yyyy()} | {LastAction?.ExchangeRateDate.dd_MM_yyyy()}) {statusText}");

            // Saat uygunsa ve bugüne ait döviz bilgileri alınmadıysa alınır ve kaydedilir
            await ReadAndSaveIfNot(now, AuditType.Daily, cancellationToken);
        }

        private async Task ReadAndSaveIfNot(DateTimeOffset exchangeRateDate, AuditType auditType, CancellationToken cancellationToken = default)
        {
            try
            {
                var count = await _internalExchangeRateService.ReadAndSaveIfNot(exchangeRateDate, cancellationToken);

                AuditFileModel audit = new()
                {
                    AuditDate = DateTimeOffset.Now,
                    AuditStatus = AuditStatus.Success,
                    AuditType = auditType,
                    ExchangeRateDate = exchangeRateDate,
                    Message = $"{count} adet veri BAŞARIYLA kaydedildi.",
                };

                LastAction = audit;
                await audit.AddAuditLine(_settings);

            }
            catch (Exception ex)
            {
                var innerText = ex.InnerException != null
                    ? string.Format(", InnerException = {0}", ex.InnerException.Message)
                    : null;

                AuditFileModel audit = new()
                {
                    AuditDate = DateTimeOffset.Now,
                    AuditStatus = AuditStatus.Error,
                    AuditType = auditType,
                    ExchangeRateDate = exchangeRateDate,
                    Message = $"Error = {ex.Message}{innerText}",
                };

                LastAction = audit;
                await audit.AddAuditLine(_settings);
                throw;
            }
        }


        /// <summary>
        /// Audit dosyasından son satır bilgisini getirerek <see cref="LastAction"/> nesnesini doldurur
        /// </summary>
        protected async Task SetLastActionFromAuditFile(CancellationToken cancellationToken) => LastAction = await AuditFileExtensions.GetLastAudit(_settings, cancellationToken);

        /// <summary>
        /// Audit dosyasını oluşturur (Eğer aktif ise)
        /// </summary>
        /// <returns></returns>
        protected async Task CreateAuditFile() => await AuditFileExtensions.CreateAuditFile(_settings);

        /// <summary>
        /// Çalışma saati kotrolünü yapıp yapmayacağını belirler.
        /// Eğer true ise çalışma saati kontrolü yapılmamalıdır
        /// False ise yapılmalıdır.
        /// </summary>
        private static bool SkipWorkingHour
            => Environment.GetEnvironmentVariable(EnvironmentNames.ExchangeRateFactory_Skip_WorkingHour)?.Equals("true", comparisonType: StringComparison.InvariantCultureIgnoreCase) == true;
    }
}
