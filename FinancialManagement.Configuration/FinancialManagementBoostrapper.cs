using FinancialManagement.Application;
using FinancialManagement.Application.Contracts.Fund;
using FinancialManagement.Application.Contracts.PettyCash;
using FinancialManagement.Domain.FundAgg;
using FinancialManagement.Domain.PettyCashAgg;
using FinancialManagement.Domain.ReceiptsPaymentAgg;
using FinancialManagement.Infrastructure.EFCore;
using FinancialManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FinancialManagement.Configuration
{
    public class FinancialManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IFundRepository, FundRepository>();
            services.AddTransient<IFundApplication, FundApplication>();

            services.AddTransient<IPettyCashRepository, PettyCashRepository>();
            services.AddTransient<IPettyCashApplication, PettyCashApplication>();

            services.AddTransient<IReceiptsPaymentRepository, ReceiptsPaymentRepository>();
            services.AddTransient<IReceiptsPaymentApplication, ReceiptsPaymentApplication>();

            services.AddDbContext<FinancialSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<FinancialFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
