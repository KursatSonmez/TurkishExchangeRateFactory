using ExchangeRateFactory.Common.Extensions;
using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Services.Internal.Expressions;
using ExchangeRateFactory.Factory.Services.Public.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Factory.Services.Public
{
    public class ExchangeRateService<T, PK> : IExchangeRateService<T, PK>
        where T : ExchangeRate<PK>
        where PK : struct
    {
        private readonly DbContext _context;
        private readonly ExchangeRateExpressions<T, PK> _expressions;
        public ExchangeRateService(
            DbContext context,
            ExchangeRateExpressions<T, PK> expressions
            )
        {
            _context = context;
            _expressions = expressions;
        }

        public async Task<Dtos.ExchangeRateSummary<PK>[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, string[] currencyCodes = null, CancellationToken cancellationToken = default)
        {
            return await GetTodayOrSpecificDate(
                x => new Dtos.ExchangeRateSummary<PK>
                {
                    CurrencyCode = x.CurrencyCode,
                    BanknoteSelling = x.BanknoteSelling,
                    Id = x.Id,
                },
            specificDate,
            currencyCodes,
            cancellationToken);
        }

        public async Task<TSelectResult[]> GetTodayOrSpecificDate<TSelectResult>(Expression<Func<ExchangeRate<PK>, TSelectResult>> selector, DateTimeOffset? specificDate = null, string[] currencyCodes = null, CancellationToken cancellationToken = default)
        {
            var date = specificDate ?? DateTimeOffset.Now;

            Expression<Func<T, bool>> exp = _expressions.GetSelectExpression(date);

            if (currencyCodes != null && currencyCodes.Length > 1)
                exp = exp.AndAlso(x => currencyCodes.Contains(x.CurrencyCode));

            return await DbSet
                .Where(exp)
                .OrderBy(x => x.CurrencyCode)
                .Select(selector)
                .ToArrayAsync(cancellationToken);
        }

        private DbSet<T> DbSet => _context.Set<T>();
    }
}
