using CodeManagement.Application;
using CodeManagement.Application.Contracts.Code;
using CodeManagement.Configuration;
using GeneralInfoManagement.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PersonManagement.Application;
using PersonManagement.Application.Contract.Persons;
using PersonManagement.Configuration;
using PersonManagement.Domain.Person.PersonAgg;
using PersonManagement.Infrastructure.EFCore;
using PersonManagement.Infrastructure.EFCore.Repository;
using System.Linq;

using System.Windows;
namespace Taadol
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }
        public static string ConnectionString { get; private set; }
        protected override void OnStartup(StartupEventArgs e)
        {

            var services = new ServiceCollection();

            string connectionString =
                @"Data Source=DESKTOP-MRP0FEV\MSSQLSERVER86;Initial Catalog=TaadolFake;Integrated Security=True;TrustServerCertificate=True";
            ConnectionString = connectionString;
            services.AddTransient<ICodeGeneratorService, CodeGeneratorService>();

            CodeManagementBoostrapper.Configure(services, connectionString);
            GeneralInfoManagementBoostrapper.Configure(services, connectionString);
            PersonManagementBoostrapper.Configure(services, connectionString);

            services.AddTransient<IPersonRepository, PersonRepository>();
            services.AddTransient<IPersonApplication, PersonApplication>();
            services.AddDbContext<PersonSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<PersonFakeDataContext>(x => x.UseSqlServer(connectionString));

            ServiceProvider = services.BuildServiceProvider();

            base.OnStartup(e);
        }
    }
}