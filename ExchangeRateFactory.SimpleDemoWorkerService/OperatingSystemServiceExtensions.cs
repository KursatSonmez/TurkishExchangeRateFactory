using Microsoft.Extensions.Hosting;
using System;
using System.Runtime.InteropServices;

namespace ExchangeRateFactory.SimpleDemoWorkerService
{
    public static class OperatingSystemServiceExtensions
    {
        public static IHostBuilder UseOperatingSystemService(this IHostBuilder builder)
        {
            if (GetOperatingSystem() == OSPlatform.Windows)
                return builder.UseWindowsService();
            else
                return builder.UseSystemd();
        }

        public static OSPlatform GetOperatingSystem()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                return OSPlatform.OSX;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return OSPlatform.Linux;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return OSPlatform.Windows;
            }

            throw new Exception("Cannot determine operating system!");
        }
    }
}
