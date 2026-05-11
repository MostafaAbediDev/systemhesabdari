using FixedAssetManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FixedAssetManagement.Configuration
{
    public class FixedAssetManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<FixedAssetSystemContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
