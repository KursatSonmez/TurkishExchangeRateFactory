using ExchangeRateFactory.Demo.Data.DataContext;
using ExchangeRateFactory.Demo.Data.Entities;
using ExchangeRateFactory.Worker.Public.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;

namespace ExchangeRateFactory.Demo
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = CreateHostBuilder(args);
            var host = builder.Build();

            using var scope = host.Services.CreateScope();

            using var seeder = new DataSeeder(
                scope.ServiceProvider.GetRequiredService<ExchangeRateFactoryDbContext>()
                );

            await seeder.Seed();

            await host.RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    ConfigureServices(hostContext, services);

                });

        public static IServiceCollection ConfigureServices(HostBuilderContext hostContext, IServiceCollection services)
        {
            string connectionStr = hostContext.Configuration.GetConnectionString("DBContext");
            services.AddDbContext<ExchangeRateFactoryDbContext>(x =>
            {
                //x.UseInMemoryDatabase("InMemory");
                x.UseSqlServer(connectionStr);

                if (hostContext.HostingEnvironment.IsDevelopment())
                    x.EnableSensitiveDataLogging();
            });

            var workingHour = hostContext.Configuration["WorkingHour"];

            services.UseExchangeRateFactoryWorker<ExchangeRateFactoryDbContext, ExchangeRate, Guid>(x =>
            {
                if (string.IsNullOrWhiteSpace(workingHour) == false)
                    x.WorkingHour = workingHour;
                //x.AuditIsActive = false;
                return x;
            });

            return services;
        }
    }
}
