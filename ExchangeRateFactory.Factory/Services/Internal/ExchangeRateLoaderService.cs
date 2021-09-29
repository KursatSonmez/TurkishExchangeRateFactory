using ExchangeRateFactory.Common.Extensions;
using ExchangeRateFactory.Data.Entities;
using ExchangeRateFactory.Factory.Services.Internal.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;

namespace ExchangeRateFactory.Factory.Services.Internal
{
    public class ExchangeRateLoaderService : IExchangeRateLoaderService
    {
        private readonly ILogger<ExchangeRateLoaderService> _logger;

        public ExchangeRateLoaderService(
            ILogger<ExchangeRateLoaderService> logger
            )
        {
            _logger = logger;
        }

        /// <summary>
        /// Maksimum deneme sayısı
        /// Sistem eğer request esnasında bir hata yaşarsa maksimum bu sayı kadar denemeye yapacak.
        /// Eğer deneme sayısı bu sayıdan büyük ise deneme yapılmayacak.
        /// </summary>
        private const byte WebXmlPageMaxAttempt = 5;

        public async Task<XmlDocument> LoadXml(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
            => await ReadWebXmlPage(specificDate, cancellationToken: cancellationToken);

        public async Task<T[]> LoadExchangeRate<T, PK>(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
            where T : ExchangeRate<PK>, new()
            where PK : struct
        {
            var xmlDocument = await LoadXml(specificDate, cancellationToken);

            return ConvertXmlToExchangeRate<T, PK>(xmlDocument, specificDate, cancellationToken);
        }

        /// <summary>
        /// XML Dökümanını okur ve içindeki bilgileri IExchangeRate olarak geri döner
        /// </summary>
        /// <typeparam name="PK"></typeparam>
        /// <param name="xmlDocument"></param>
        /// <param name="specificDate">
        /// ExchangeRateDate değerinin ne olduğunu belirlemek için kullanılır.
        /// Yani verilerin hangi tarihe ait olduğunu temsil eder.
        /// 
        /// Eğer null ise DateTime.Now değerini alır.
        /// </param>
        private static T[] ConvertXmlToExchangeRate<T, PK>(XmlDocument xmlDocument, DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
            where T : ExchangeRate<PK>, new()
            where PK : struct
        {
            var list = new System.Collections.Concurrent.ConcurrentBag<T>();

            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Tarih_Date");
            foreach (XmlNode node in nodeList)
            {
                DateTimeOffset tarih_ddMMyyyy = Convert.ToDateTime(node.Attributes.GetNamedItem("Tarih").Value, CultureTr);

                string bulletinNo = node.Attributes.GetNamedItem("Bulten_No").Value;

                var po = new ParallelOptions() { MaxDegreeOfParallelism = System.Environment.ProcessorCount, CancellationToken = cancellationToken };

                Parallel.ForEach(node.Cast<XmlNode>(), po, (child) =>
                {
                    XmlNode unit = child["Unit"];
                    XmlNode isim = child["Isim"];
                    XmlNode currencyName = child["CurrencyName"];
                    XmlNode forexBuying = child["ForexBuying"];
                    XmlNode forexSelling = child["ForexSelling"];
                    XmlNode banknoteBuying = child["BanknoteBuying"];
                    XmlNode banknoteSelling = child["BanknoteSelling"];
                    XmlNode crossRateUSD = child["CrossRateUSD"];
                    XmlNode crossRateOther = child["CrossRateOther"];

                    var res = new T()
                    {
                        CreateDate = DateTimeOffset.Now,
                        UpdateDate = DateTimeOffset.Now,
                        Kod = child.Attributes.GetNamedItem("Kod").Value,
                        CurrencyCode = child.Attributes.GetNamedItem("CurrencyCode").Value,

                        ExchangeRateDate = specificDate?.DateTime ?? DateTime.Now,
                        ReleaseDate = tarih_ddMMyyyy.DateTime,
                        Unit = ToInt(unit),
                        ForexBuying = ToDecimal(forexBuying).Value,
                        ForexSelling = ToDecimal(forexSelling).Value,
                        BanknoteBuying = ToDecimal(banknoteBuying).Value,
                        BanknoteSelling = ToDecimal(banknoteSelling).Value,
                        CurrencyName = currencyName.InnerText,
                        BulletinNo = bulletinNo,
                        CrossRateOther = ToDecimal(crossRateOther, true),
                        CrossRateUSD = ToDecimal(crossRateUSD, true),
                        Isim = isim.InnerText,
                    };

                    if (typeof(PK) == typeof(Guid))
                        res.Id = (PK)System.ComponentModel.TypeDescriptor.GetConverter(typeof(PK)).ConvertFromInvariantString(Guid.NewGuid().ToString());

                    list.Add(res);
                });
            }

            return list.ToArray();
        }


        private async Task<XmlDocument> ReadWebXmlPage(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
        {
            string url = GetUrl(specificDate);

            int i = 1;
            while (i <= WebXmlPageMaxAttempt)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    // eğer başarıyla okunduysa xml bilgisi döner
                    return await GetXMLDocumentFromWebPage(url, cancellationToken);
                }
                catch (Exception ex)
                {
                    string mess = "Kur bilgisi okunamadı!";
                    if (specificDate.HasValue)
                        mess += $" specificDate = {specificDate.Value.dd_MM_yyyy()}  URL = {url}";
                    else
                        mess += $" (Günlük)  URL = {url}";

                    if (i + 1 <= WebXmlPageMaxAttempt)
                        mess += $"   Tekrar denenecek... (i = {i}/{WebXmlPageMaxAttempt})";

                    _logger.LogError(ex, mess);

                    cancellationToken.ThrowIfCancellationRequested();

                    // okunamadıysa 10 sn bekler
                    Thread.Sleep(10000);
                    ++i;
                }
            }
            throw new Exception($"Kur bilgisi okunamadı! Maksimum deneme sayısına ulaşıldı ({WebXmlPageMaxAttempt})!!! Url = {url}");
        }


        /// <summary>
        /// İlgili TCMB web sayfasını okuyarak XML nesnesini ve hangi sayfayı okuduğuna dair bilgiyi döner.
        /// 
        /// Eğer <paramref name="date"/> parametresi belirtilmediyse bugüne ait xml bilgisini döner.
        /// 
        /// <strong>Eğer xml başarıyla okunamadıysa null döner!</strong>
        /// </summary>
        /// <param name="date">Bugün değil de belirli bir tarihe ait bilgiler alınacaksa bu parametre kullanılır</param>
        private static async Task<XmlDocument> GetXMLDocumentFromWebPage(string url, CancellationToken cancellationToken = default)
        {
            return await Task.Run(() =>
            {
                XmlDocument xmlDoc = new();
                WebRequest request = WebRequest.Create(url);

                using var response = request.GetResponse();
                xmlDoc.Load(response.GetResponseStream());

                return xmlDoc;
            }, cancellationToken);
        }


        /// <summary>
        /// Node içeriğini nullable koşuluna göre decimal değere çevirir.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="nullable">
        /// Eğer True ise -> node içeriği boşsa NULL döner
        /// False ise -> node içeriği boşsa 1 olarak döner.
        /// </param>
        /// <returns></returns>
        private static decimal? ToDecimal(XmlNode node, bool nullable = false)
        {
            if (nullable && string.IsNullOrWhiteSpace(node.InnerText))
                return null;

            string text;
            if (nullable == false && string.IsNullOrWhiteSpace(node.InnerText))
                text = "1";
            else
                text = node.InnerText;
            //throw new ArgumentNullException(nameof(node.InnerText), $"node.InnerText is null! NodeName={node.Name}");

            var ci = System.Globalization.CultureInfo.InvariantCulture.Clone() as System.Globalization.CultureInfo;
            ci.NumberFormat.CurrencyDecimalSeparator = ".";

            return decimal.Parse(text, ci);
        }

        private static int ToInt(XmlNode node)
            => string.IsNullOrWhiteSpace(node.InnerText)
            ? 0
            : Convert.ToInt32(node.InnerText);

        private static string GetUrl(DateTimeOffset? specificDate)
            => specificDate.HasValue == false || specificDate.Value.Date == DateTimeOffset.Now.Date
                ? TodayUrl
                : GetDateUrl(specificDate.Value);

        private static string TodayUrl => "https://www.tcmb.gov.tr/kurlar/today.xml";

        /// <summary>
        /// Format: http://www.tcmb.gov.tr/kurlar/yyyyMM/ddMMyyyy.xml
        /// </summary>
        private static string GetDateUrl(DateTimeOffset date)
            => string.Format("http://www.tcmb.gov.tr/kurlar/{0}{1}/{2}{1}{0}.xml",
                date.Year.ToString("0000"),
                date.Month.ToString("00"),
                date.Day.ToString("00"));

        private static System.Globalization.CultureInfo CultureTr => new System.Globalization.CultureInfo("tr-TR");
    }
}
