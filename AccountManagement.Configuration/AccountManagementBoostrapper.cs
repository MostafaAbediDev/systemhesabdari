using AccountManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace AccountManagement.Configuration
{
    public class AccountManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AccountSystemContext>(x => x.UseSqlServer(connectionString));
        }

    }
}
