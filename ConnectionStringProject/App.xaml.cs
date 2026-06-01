using AccountManagement.Configuration;
using BankManagement.Configuration;
<<<<<<< HEAD
using CodeManagement.Configuration;
using FixedAssetManagement.Configuration;
using GeneralInfoManagement.Configuration;
using GeneralInfoManagement.Infrastructure.EFCore;
using GeneralInfoManagement.Infrastructure.EFCore.Seed.GeneralSeeders;
using InventoryManagement.Configuration;
using InvoiceManagement.Configuration;
using LogManagement.Configuration;
using Microsoft.EntityFrameworkCore;
=======
using FixedAssetManagement.Configuration;
using GeneralInfoManagement.Configuration;
using InventoryManagement.Configuration;
using InvoiceManagement.Configuration;
using LogManagement.Configuration;
>>>>>>> front
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Configuration;
using PersonManagement.Configuration;
<<<<<<< HEAD
using PersonManagement.Infrastructure.EFCore;
=======
using System.Configuration;
using System.Data;
>>>>>>> front
using System.IO;
using System.Windows;

namespace ConnectionStringProject
{
<<<<<<< HEAD
=======
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
>>>>>>> front
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

<<<<<<< HEAD
        protected override async void OnStartup(StartupEventArgs e)
=======
        protected override void OnStartup(StartupEventArgs e)
>>>>>>> front
        {
            var services = new ServiceCollection();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
<<<<<<< HEAD
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
=======
                .AddJsonFile("appsettings.json")
>>>>>>> front
                .Build();

            ConfigureServices(services, configuration);

            ServiceProvider = services.BuildServiceProvider();

<<<<<<< HEAD
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

            base.OnStartup(e);
        }


        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var fakeConnectionString = configuration.GetConnectionString("TaadolFakeDb");   

            CodeManagementBoostrapper.Configure(services, fakeConnectionString);
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
=======
            base.OnStartup(e);
        }

        private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("TaadolDb");

            AccountManagementBoostrapper.Configure(services, connectionString);
            BankManagementBoostrapper.Configure(services, connectionString);
            FixedAssetManagementBoostrapper.Configure(services, connectionString);
            GeneralInfoManagementBoostrapper.Configure(services, connectionString);
            InventoryManagementBoostrapper.Configure(services, connectionString);
            InvoiceManagementBoostrapper.Configure(services, connectionString);
            LogManagementBoostrapper.Configure(services, connectionString);
            PayrollSystemManagementBoostrapper.Configure(services, connectionString);
            PersonManagementBoostrapper.Configure(services, connectionString);
        }
    }

>>>>>>> front
}
