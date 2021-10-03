using ExchangeRateFactory.Demo.Customize.DataContext;
using ExchangeRateFactory.Demo.Customize.Entities;
using ExchangeRateFactory.Factory.Services.Internal.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System;

namespace ExchangeRateFactory.UnitTests.Base
{
    public class TestBase
    {
        protected static ILogger<T> Logger<T>() => new NullLoggerFactory().CreateLogger<T>();

        protected static ExchangeRateExpressions<ExchangeRate, Guid> DefaultExpressions => ExchangeRateExpressions<ExchangeRate, Guid>.LoadDefaultValues();

        protected static ExchangeRateFactoryDbContext NewDbContext(string databaseName = null)
        {
            databaseName ??= System.Guid.NewGuid().ToString();

            var options = new DbContextOptionsBuilder<ExchangeRateFactoryDbContext>()
                .UseInMemoryDatabase(databaseName: databaseName)
                .Options;
            return ExchangeRateFactoryDbContext.Create(options);
        }

        protected virtual string GetDatabaseName([System.Runtime.CompilerServices.CallerMemberName] string methodName = null)
            => string.Format("{0}.{1}", this.GetType().Name, methodName);
    }
}
