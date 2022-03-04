using ExchangeRateFactory.Demo.Customize.Entities;
using ExchangeRateFactory.Demo.Customize.Interfaces;
using ExchangeRateFactory.Demo.Customize.Services;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExchangeRateFactory.Demo.Customize.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseExchangeRates<TContext>(this IServiceCollection services, Func<IFactorySettings, IFactorySettings> settings = null)
            where TContext : IExchangeRateFactoryDbContext
        {
            services.AddScoped<IExchangeRateService, ExchangeRateService>();

            return ExchangeRateFactory.Worker.Public.DependencyInjection.ServiceCollectionExtensions
                .UseExchangeRateFactoryWorker<TContext, ExchangeRate, Guid>(services, settings);
        }
        public static IServiceCollection AddExchangeRateBackgroundService<TContext>(this IServiceCollection services)
            where TContext : IExchangeRateFactoryDbContext
        {
            ExchangeRateFactory.Worker.Public.DependencyInjection.ServiceCollectionExtensions
                   .AddExchangeRateFactoryBackgroundService<TContext, ExchangeRate, Guid>(services);

            return services;
        }
    }
}
