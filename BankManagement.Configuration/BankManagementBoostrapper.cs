using BankManagement.Application;
using BankManagement.Application.Contracts.Bank;
using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Infrastructure.EFCore;
using BankManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagement.Configuration
{
    public class BankManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IBankRepository, BankRepository>();
            services.AddTransient<IBankApplication, BankApplication>();


            services.AddDbContext<BankSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<BankFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
