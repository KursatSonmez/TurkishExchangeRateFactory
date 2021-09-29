using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Data.Interfaces;
using ExchangeRateFactory.Factory.Services.Internal.Expressions;
using ExchangeRateFactory.Factory.Utilities;
using ExchangeRateFactory.Factory.Utilities.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace ExchangeRateFactory.Factory.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection UseExchangeRateFactoryServices<TContext, T, ExchangeRatePK>(this IServiceCollection services, Func<IFactorySettings, IFactorySettings> settings = null, Func<ExchangeRateExpressions<T, ExchangeRatePK>, ExchangeRateExpressions<T, ExchangeRatePK>> expressions = null)
            where T : ExchangeRate<ExchangeRatePK>
            where TContext : IExchangeRateFactoryDbContext<T, ExchangeRatePK>
            where ExchangeRatePK : struct
        {
            var defaultSettings = FactorySettings.LoadDefaultValues();
            services.AddSingleton<IFactorySettings>(x => settings?.Invoke(defaultSettings) ?? defaultSettings);

            var defaultExpressions = ExchangeRateExpressions<T, ExchangeRatePK>.LoadDefaultValues();
            services.AddSingleton(x => expressions?.Invoke(defaultExpressions) ?? defaultExpressions);

            services.AddScoped(typeof(DbContext), typeof(TContext));

            services.AddScoped(typeof(IExchangeRateFactoryDbContext<T, ExchangeRatePK>), x => x.GetRequiredService<TContext>());

            // Internal
            services.AddScoped<Services.Internal.Interfaces.IExchangeRateLoaderService, Services.Internal.ExchangeRateLoaderService>();
            services.AddScoped(typeof(Services.Internal.Interfaces.IExchangeRateService<,>), typeof(Services.Internal.ExchangeRateService<,>));


            // Public
            services.AddScoped(typeof(Services.Public.Interfaces.IExchangeRateService<,>), typeof(Services.Public.ExchangeRateService<,>));

            return services;
        }
    }
}
