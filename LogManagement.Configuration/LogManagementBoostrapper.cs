using LogManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace LogManagement.Configuration
{
    public class LogManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<LogSystemContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
