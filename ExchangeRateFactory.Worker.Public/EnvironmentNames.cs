
namespace ExchangeRateFactory.Worker.Public
{
    internal static class EnvironmentNames
    {
        /// <summary>
        /// Çalışma saati kotrolünü yapıp yapmayacağını belirler.
        /// Eğer Environmentler arasında bu environment varsa ve değeri true ise,
        /// çalışma saati kontrolü yapılmaz.
        /// False ise çalışma saati kontrol edilir. Eğer saat uygunsa işlemler gerçekleştirilir, değilse gerçekleştirilmez.
        /// </summary>
        internal const string ExchangeRateFactory_Skip_WorkingHour = "ExchangeRateFactory_Skip_WorkingHour";
    }
}
