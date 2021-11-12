# Turkish Lira Exchange Rate Factory
This library is used to collects and automatically save to database foreign currency informations for the Turkish Lira from the CBRT (TCMB) XML Web site.
The system runs as a background service.

## Description

The system will start collects to data from CBRT at a certain time of day and then save those data to the database.
First, the system checks the working hour. If the system hour equal to the `WorkingHour`, the system will start reading the data of that day and save those to the database.
(Default `WorkingHour` value is `00`. You can review the [appsettings.Development.json file in ExchangeRateFactory.Demo](ExchangeRateFactory.Demo/appsettings.Development.json) for change.)


## Quick Start

1 - Implement interface and model mapping. <i>(Please do not forget to update the database after these implementations.)</i>

~~~ c#
using ExchangeRateFactory.Data.Interfaces;
using ExchangeRateFactory.Data.ModelBuilderExtensions;

public class SimpleDemoDbContext : DbContext, IExchangeRateFactoryDbContext<ExchangeRate, int>
{
    public DbSet<ExchangeRate> ExchangeRates { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseExchangeRateFactory<int>();
    }
}
~~~

2 - Implement services and workers.

~~~ c#
using ExchangeRateFactory.Worker.Public.DependencyInjection;


public static IServiceCollection ConfigureServices(HostBuilderContext hostContext, IServiceCollection services) {
    services.UseExchangeRateFactoryWorker<SimpleDemoDbContext, ExchangeRate, int>(x => x);
}
~~~

3 - Usage

~~~ c#
using ExchangeRateFactory.Factory.Dtos;
using ExchangeRateFactory.Factory.Services.Public.Interfaces;

private readonly IExchangeRateService<ExchangeRate, int> _exchangeRateService;
public ExchangeRateService(IExchangeRateService<ExchangeRate, int> exchangeRateService)
{
    _exchangeRateService = exchangeRateService;
}

public async Task<ExchangeRateSummary<int>[]> GetTodayOrSpecificDate(DateTimeOffset? specificDate = null, CancellationToken cancellationToken = default)
{
    return await _exchangeRateService.GetTodayOrSpecificDate(
        specificDate: specificDate,
        cancellationToken: cancellationToken);
}
~~~

After the above implementations, the system will start working as a background service.


## Notes

For more, you can check out to`ExchangeRateFactory.SimpleDemo` or `ExchangeRateFactory.Demo` projects
