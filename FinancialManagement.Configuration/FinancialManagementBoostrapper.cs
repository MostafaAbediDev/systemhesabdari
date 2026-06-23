using FinancialManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialManagement.Configuration
{
    public class FinancialManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FinancialSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<FinancialFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
