using AccountManagement.Configuration;
using BankManagement.Configuration;
using FixedAssetManagement.Configuration;
using GeneralInfoManagement.Configuration;
using GeneralInfoManagement.Infrastructure.EFCore;
using GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders;
using InventoryManagement.Configuration;
using InvoiceManagement.Configuration;
using LogManagement.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Configuration;
using PersonManagement.Configuration;
using System.IO;
using System.Windows;

namespace ConnectionStringProject
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override async void OnStartup(StartupEventArgs e)
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            ConfigureServices(services, configuration);

            ServiceProvider = services.BuildServiceProvider();

            using (var scope = ServiceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<GeneralInfoFakeDataContext>();

                await context.Database.MigrateAsync();

                // ✅ اجرای Seed بعد از Migration
                await GeneralSeeders.SeedAsync(context);
            }

            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var fakeConnectionString = configuration.GetConnectionString("TaadolFakeDb");

            AccountManagementBoostrapper.Configure(services, fakeConnectionString);
            BankManagementBoostrapper.Configure(services, fakeConnectionString);
            FixedAssetManagementBoostrapper.Configure(services, fakeConnectionString);
            GeneralInfoManagementBoostrapper.Configure(services, fakeConnectionString);
            InventoryManagementBoostrapper.Configure(services, fakeConnectionString);
            InvoiceManagementBoostrapper.Configure(services, fakeConnectionString);
            LogManagementBoostrapper.Configure(services, fakeConnectionString);
            PayrollSystemManagementBoostrapper.Configure(services, fakeConnectionString);
            PersonManagementBoostrapper.Configure(services, fakeConnectionString);
        }
    }
}
