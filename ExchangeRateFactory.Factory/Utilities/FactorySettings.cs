using ExchangeRateFactory.Factory.Utilities.Interfaces;
using System;
using System.IO;

namespace ExchangeRateFactory.Factory.Utilities
{
    public class FactorySettings : IFactorySettings
    {
        public FactorySettings()
        {
        }

        public string AuditFilePath { get; set; }

        public string AuditFileName { get; set; }

        public bool AuditIsActive { get; set; }

        private string _WorkingHour;
        public string WorkingHour
        {
            get => _WorkingHour;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentNullException(nameof(WorkingHour));

                //if (value.Contains(":") == false && value.Length == 2)
                //    value += ":00";

                if (value.Length != 2)
                    throw new ArgumentException("Must have 2 characters!",nameof(WorkingHour));

                if (int.TryParse(value, out int res) == false || (res < 0 || res > 23))
                    throw new ArgumentOutOfRangeException(nameof(WorkingHour), "It can only be in the range [00-23]");

                // 00:00  veya 23:50 vs. gibi format kontrolü yapılır.
                //if (Regex.IsMatch(value, "^([0-1]?[0-9]|2[0-3]):[0-5][0-9]$") == false)
                //    throw new ArgumentException("Format invalid!", nameof(WorkingHour));

                _WorkingHour = value;
            }
        }

        public static FactorySettings LoadDefaultValues()
        {
            var assemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            return new()
            {
                AuditFileName = "audit_exchange_rate.txt",

                AuditFilePath = Path.Combine(Path.GetDirectoryName(assemblyLocation), "audit_exchange_rate.txt"),

                AuditIsActive = true,

                WorkingHour = "00",
            };
        }
    }
}
