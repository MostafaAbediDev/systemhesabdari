using AccountManagement.Configuration;
using BankManagement.Configuration;
using CodeManagement.Configuration;
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
using PersonManagement.Infrastructure.EFCore;
using PersonManagement.Infrastructure.EFCore.Seed;
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

            using var scope = ServiceProvider.CreateScope();
            var sp = scope.ServiceProvider;

            // =========================
            // 1. Migrate Databases
            // =========================

            var generalContext =
                sp.GetRequiredService<GeneralInfoFakeDataContext>();

            var personContext =
                sp.GetRequiredService<PersonFakeDataContext>();

            await generalContext.Database.MigrateAsync();
            await personContext.Database.MigrateAsync();

            // =========================
            // 2. Seed General Info
            // =========================

            await GeneralSeeders.SeedAsync(generalContext);

            // =========================
            // 3. Seed Person (Fake Data)
            // =========================

            var personSeeder =
                sp.GetRequiredService<PersonFakeSeeder>();

            await personSeeder.SeedAsync();

            base.OnStartup(e);
        }


        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var fakeConnectionString = configuration.GetConnectionString("TaadolFakeDb");   
            var ConnectionString = configuration.GetConnectionString("TaadolDb");

            CodeManagementBoostrapper.Configure(services, ConnectionString);

            AccountManagementBoostrapper.Configure(services, fakeConnectionString);
            BankManagementBoostrapper.Configure(services, fakeConnectionString);
            FixedAssetManagementBoostrapper.Configure(services, fakeConnectionString);
            GeneralInfoManagementBoostrapper.Configure(services, fakeConnectionString);
            InventoryManagementBoostrapper.Configure(services, fakeConnectionString);
            InvoiceManagementBoostrapper.Configure(services, fakeConnectionString);
            LogManagementBoostrapper.Configure(services, fakeConnectionString);
            PayrollSystemManagementBoostrapper.Configure(services, fakeConnectionString);
            PersonManagementBoostrapper.Configure(services, fakeConnectionString);

            services.AddScoped<PersonFakeSeeder>();

        }
    }
}
