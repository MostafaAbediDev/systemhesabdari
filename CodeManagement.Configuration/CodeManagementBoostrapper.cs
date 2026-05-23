using CodeManagement.Application;
using CodeManagement.Application.Contracts.Code;
using CodeManagement.Domain.CodeAgg;
using CodeManagement.Infrastructure.EFCore;
using CodeManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace CodeManagement.Configuration
{
    public class CodeManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {

            services.AddTransient<ICodeRepository, CodeRepository>();
            services.AddTransient<ICodeApplication, CodeApplication>();

            services.AddDbContext<CodeSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<CodeFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
