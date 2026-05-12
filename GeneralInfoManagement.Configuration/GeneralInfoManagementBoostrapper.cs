using GeneralInfoManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralInfoManagement.Configuration
{
    public class GeneralInfoManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<GeneralInfoSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<GeneralInfoFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
