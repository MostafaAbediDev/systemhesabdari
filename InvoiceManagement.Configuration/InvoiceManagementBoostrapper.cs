using InvoiceSystemManagement.Infrastructure.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceManagement.Configuration
{
    public class InvoiceManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddDbContext<InvoiceSystemContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
