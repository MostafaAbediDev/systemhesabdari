using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Infrastructure.EFCore;

namespace PayrollSystemManagement.Configuration
{
    public class PayrollSystemManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<PayrollSystemContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
