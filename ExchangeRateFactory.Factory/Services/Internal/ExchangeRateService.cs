using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Services.Internal.Expressions;
using ExchangeRateFactory.Factory.Services.Internal.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Factory.Services.Internal
{
    public class ExchangeRateService<T, PK> : IExchangeRateService<T, PK>
        where T : ExchangeRate<PK>, new()
        where PK : struct
    {
        private readonly IExchangeRateLoaderService _loaderService;
        private readonly DbContext _context;
        private readonly ExchangeRateExpressions<T, PK> _expressions;
        public ExchangeRateService(
            IExchangeRateLoaderService loaderService,
            DbContext dbContext,
            ExchangeRateExpressions<T, PK> expressions
            )
        {
            _loaderService = loaderService;
            _context = dbContext;
            _expressions = expressions;
        }

        public async Task<int> ReadAndSaveIfNot(DateTimeOffset exchangeRateDate, CancellationToken cancellationToken = default)
        {
            // Eğer bu tarihe ait veri varsa herhangi bir işlem yapılmaz
            if (await this.AnyAsync(exchangeRateDate, cancellationToken))
                return 0;

            var list = await _loaderService.LoadExchangeRate<T, PK>(exchangeRateDate, cancellationToken);

            list
                .AsParallel()
                .ForAll(item =>
                {
                    item.OnInsert();
                });

            await DbSet.AddRangeAsync(list, cancellationToken: cancellationToken);

            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<bool> AnyAsync(DateTimeOffset exchangeRateDate, CancellationToken cancellationToken = default)
        {
            return await DbSet.AnyAsync(_expressions.GetSelectExpression(exchangeRateDate), cancellationToken);
        }

        private DbSet<T> DbSet => _context.Set<T>();
    }
}
