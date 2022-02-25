using ExchangeRateFactory.SimpleDemo.Data;
using ExchangeRateFactory.SimpleDemo.DataContext;
using ExchangeRateFactory.Worker.Public.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ExchangeRateFactory.SimpleDemoWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseOperatingSystemService()
                .ConfigureServices((hostContext, services) =>
                {
                    string connectionStr = hostContext.Configuration.GetConnectionString("DBContext");
                    services.AddDbContext<SimpleDemoDbContext>(x =>
                    {
                        x.UseSqlServer(connectionStr, x => x.MigrationsAssembly(typeof(SimpleDemoDbContext).Assembly.FullName));

                        if (hostContext.HostingEnvironment.IsDevelopment())
                            x.EnableSensitiveDataLogging();
                    });

                    services.UseExchangeRateFactoryWorker<SimpleDemoDbContext, ExchangeRate, int>(x =>
                    {
                        x.WorkingHour = DateTimeOffset.Now.Hour.ToString("00");
                        return x;
                    })
                    .AddExchangeRateFactoryHostedService<SimpleDemoDbContext, ExchangeRate, int>();
                });
    }
}
