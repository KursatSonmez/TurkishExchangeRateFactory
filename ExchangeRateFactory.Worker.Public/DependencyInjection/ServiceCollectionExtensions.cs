using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Data.Interfaces;
using ExchangeRateFactory.Factory.DependencyInjection;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using ExchangeRateFactory.Worker.Public.BackgroundServices;
using ExchangeRateFactory.Worker.Public.Workers;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExchangeRateFactory.Worker.Public.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseExchangeRateFactoryWorker<TContext, T, ExchangeRatePK>(this IServiceCollection services, Func<IFactorySettings, IFactorySettings> settings = null)
            where T : ExchangeRate<ExchangeRatePK>
            where TContext : IExchangeRateFactoryDbContext<T, ExchangeRatePK>
            where ExchangeRatePK : struct
        {
            services.UseExchangeRateFactoryServices<TContext, T, ExchangeRatePK>(settings);

            services.AddScoped(typeof(ExchangeRateFactoryWorker<,>));

            services.AddHostedService<ExchangeRateFactoryBackgroundService<T, ExchangeRatePK>>();

            return services;
        }
    }
}
