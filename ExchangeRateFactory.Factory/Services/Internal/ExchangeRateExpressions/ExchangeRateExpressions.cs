using ExchangeRateFactory.Data.Entities;
using System;
using System.Linq.Expressions;

namespace ExchangeRateFactory.Factory.Services.Internal.Expressions
{
    public class ExchangeRateExpressions<T, PK>
        where T : ExchangeRate<PK>
        where PK : struct
    {
        /// <summary>
        /// Select işlemi yaparken kullanılacak Where koşulunu temsil eder
        /// </summary>
        /// <param name="date">Hangi tarihe ait verilerin getirileceğini belirlemek için kullanılır</param>
        public Func<DateTimeOffset, Expression<Func<T, bool>>> GetSelectExpression;


        public static ExchangeRateExpressions<T, PK> LoadDefaultValues()
            => new()
            {
                GetSelectExpression = (date) => x => x.ExchangeRateDate.Date == date.Date,
            };
    }
}
