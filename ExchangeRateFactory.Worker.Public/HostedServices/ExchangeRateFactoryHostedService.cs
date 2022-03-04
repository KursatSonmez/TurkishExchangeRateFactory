using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Worker.Public.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Worker.Public.HostedServices
{
    public class ExchangeRateFactoryHostedService<T, PK> : IHostedService, IDisposable
        where T : ExchangeRate<PK>
        where PK : struct
    {
        private readonly ILogger<ExchangeRateFactoryHostedService<T, PK>> _logger;

        private Timer _timer = null;

        public ExchangeRateFactoryHostedService(
            ILogger<ExchangeRateFactoryHostedService<T, PK>> logger,
            IServiceProvider serviceProvider
            )
        {
            _logger = logger;
            Services = serviceProvider;
        }

        public readonly IServiceProvider Services;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("ExchangeRateFactoryHostedService running at: {time}", DateTimeOffset.Now);

            _timer = new Timer(
                async (e) =>
                {
                    await DoWork(cancellationToken);
                },
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(1)
                );

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed ExchangeRateFactoryHostedService is stopping.");

            _timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        protected virtual async Task DoWork(CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Consume Scoped ExchangeRateFactoryHostedService is working.");

            using var scope = Services.CreateScope();

            var worker = scope.ServiceProvider.GetRequiredService<ExchangeRateFactoryWorker<T, PK>>();

            await worker.Execute(cancellationToken);
        }


        #region IDisposable

        private void DisposeManagedResources()
        {
            _timer?.Dispose();
        }
        private void DisposeNativeResources()
        {
            _timer = null;
        }

        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed) return;
            if (disposing)
                DisposeManagedResources();

            DisposeNativeResources();
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~ExchangeRateFactoryHostedService() => Dispose(false);

        #endregion
    }
}
