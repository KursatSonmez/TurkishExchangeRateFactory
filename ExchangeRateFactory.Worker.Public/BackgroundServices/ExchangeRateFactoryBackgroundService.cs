using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using ExchangeRateFactory.Worker.Public.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Worker.Public.BackgroundServices
{
    public class ExchangeRateFactoryBackgroundService<T, PK> : BackgroundService
        where T : ExchangeRate<PK>
        where PK : struct
    {
        private readonly ILogger<ExchangeRateFactoryBackgroundService<T, PK>> _logger;
        public ExchangeRateFactoryBackgroundService(
            ILogger<ExchangeRateFactoryBackgroundService<T, PK>> logger,
            IFactorySettings factorySettings,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            Services = serviceProvider;
            FactorySettings = factorySettings;
        }

        public readonly IServiceProvider Services;
        protected readonly IFactorySettings FactorySettings;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("ExchangeRateFactoryBackgroundService running at: {time}", DateTimeOffset.Now);

                try
                {
                    await DoWork(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "");
                }

                await Task.Delay(FactorySettings.TimerPeriod, stoppingToken);
            }
        }

        protected virtual async Task DoWork(CancellationToken cancellationToken)
        {
            using var scope = Services.CreateScope();

            var worker = scope.ServiceProvider.GetRequiredService<ExchangeRateFactoryWorker<T, PK>>();

            await worker.Execute(cancellationToken);
        }
    }
}
