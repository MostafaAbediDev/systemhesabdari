using CodeManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagement.Configuration
{
    public class CodeManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<CodeSystemContext>(x => x.UseSqlServer(connectionString));
            //services.AddDbContext<GeneralInfoFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
