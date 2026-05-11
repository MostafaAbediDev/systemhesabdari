using BankManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagement.Configuration
{
    public class BankManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<BankSystemContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
