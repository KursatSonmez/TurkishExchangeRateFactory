using ExchangeRateFactory.SimpleDemo.Data;
using ExchangeRateFactory.SimpleDemo.DataContext;
using ExchangeRateFactory.Worker.Public.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;

namespace ExchangeRateFactory.SimpleDemo
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        public Startup(IConfiguration configuration, IWebHostEnvironment env)
        {
            Configuration = configuration;
            _hostingEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();

            string connectionStr = Configuration.GetConnectionString("DBContext");
            services.AddDbContext<SimpleDemoDbContext>(x =>
            {
                //x.UseInMemoryDatabase("InMemory");
                x.UseSqlServer(connectionStr, x => x.MigrationsAssembly(typeof(SimpleDemoDbContext).Assembly.FullName));

                if (_hostingEnvironment.IsDevelopment())
                    x.EnableSensitiveDataLogging();
            });

            services.AddScoped<ExchangeRateService>();

            services.UseExchangeRateFactoryWorker<SimpleDemoDbContext, ExchangeRate, int>(x =>
            {
                x.WorkingHour = DateTimeOffset.Now.Hour.ToString("00");
                return x;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
