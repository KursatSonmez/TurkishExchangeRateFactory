﻿using System;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("ExchangeRateFactory.Worker.Public")]
namespace ExchangeRateFactory.Common.Extensions
{
    internal static class DateTimeExtensions
    {
        /// <summary>
        /// Tarihi dd-MM-yyyy formatında string olarak geri döndürür
        /// </summary>
        internal static string dd_MM_yyyy(this DateTimeOffset source) => source.ToString("dd-MM-yyyy");

        /// <summary>
        /// Tarihi dd-MM-yyyy formatında string olarak geri döndürür
        /// Eğer değeri yok ise, null dönecektir.
        /// </summary>
        internal static string dd_MM_yyyy(this DateTimeOffset? source)
            => source.HasValue == false
            ? null
            : source.Value.ToString("dd-MM-yyyy");

        internal static string dd_MM_yyyy_HH_mm(this DateTimeOffset source) => source.ToString("dd-MM-yyyy HH:mm");

        internal static string dd_MM_yyyy_HH_mm(this DateTimeOffset? source)
            => source.HasValue == false
            ? null
            : source.Value.ToString("dd-MM-yyyy HH:mm");

        internal static string dd_MM_yyyy_HH_mm_ss(this DateTimeOffset source) => source.ToString("dd-MM-yyyy HH:mm:ss");

        internal static string dd_MM_yyyy_HH_mm_ss(this DateTimeOffset? source)
        => source.HasValue == false
            ? null
            : source.Value.ToString("dd-MM-yyyy HH:mm:ss");

    }
}
