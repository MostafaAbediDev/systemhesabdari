using AccountManagement.Configuration;
using BankManagement.Configuration;
using FixedAssetManagement.Configuration;
using GeneralInfoManagement.Configuration;
using InventoryManagement.Configuration;
using InvoiceManagement.Configuration;
using LogManagement.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Configuration;
using PersonManagement.Configuration;
using System.Configuration;
using System.Data;
using System.IO;
using System.Windows;

namespace Taadol_Cal
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        //public static IServiceProvider ServiceProvider { get; private set; }

        //protected override void OnStartup(StartupEventArgs e)
        //{
        //    var services = new ServiceCollection();

        //    var configuration = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json")
        //        .Build();

        //    ConfigureServices(services, configuration);

        //    ServiceProvider = services.BuildServiceProvider();

        //    base.OnStartup(e);
        //}

        //private void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        //{
        //    var connectionString = configuration.GetConnectionString("TaadolDb");

        //    AccountManagementBoostrapper.Configure(services, connectionString);
        //    BankManagementBoostrapper.Configure(services, connectionString);
        //    FixedAssetManagementBoostrapper.Configure(services, connectionString);
        //    GeneralInfoManagementBoostrapper.Configure(services, connectionString);
        //    InventoryManagementBoostrapper.Configure(services, connectionString);
        //    InvoiceManagementBoostrapper.Configure(services, connectionString);
        //    LogManagementBoostrapper.Configure(services, connectionString);
        //    PayrollSystemManagementBoostrapper.Configure(services, connectionString);
        //    PersonManagementBoostrapper.Configure(services, connectionString);
        //}
    }

}
