using CodeManagement.Infrastructure.EFCore.Repository;
using GeneralInfoManagement.Application;
using GeneralInfoManagement.Application.Contract.BranchArchice;
using GeneralInfoManagement.Application.Contract.Branches;
using GeneralInfoManagement.Application.Contract.Company;
using GeneralInfoManagement.Application.Contract.FinancialPeriod;
using GeneralInfoManagement.Application.Contract.Picture;
using GeneralInfoManagement.Domain.BaseInfo.BranchArchiveAgg;
using GeneralInfoManagement.Domain.BaseInfo.BranchesAgg;
using GeneralInfoManagement.Domain.BaseInfo.CompaniesAgg;
using GeneralInfoManagement.Domain.BaseInfo.FinancialPeriodsAgg;
using GeneralInfoManagement.Domain.BaseInfo.PictureAgg;
using GeneralInfoManagement.Infrastructure.EFCore;
using GeneralInfoManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GeneralInfoManagement.Configuration
{
    public class GeneralInfoManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IBranchArchiveRepository, BranchArchiveRepository>();
            services.AddTransient<IBranchArchiveApplication, BranchArchiveApplication>();

            services.AddTransient<IBranchRepository, BranchRepository>();
            services.AddTransient<IBranchApplication, BranchApplication>();

            services.AddTransient<IFinancialPeriodRepository, FinancialPeriodRepository>();
            services.AddTransient<IFinancialPeriodApplication, FinancialPeriodAppliction>();

            services.AddTransient<ICompanyRepository, CompanyRepository>();
            services.AddTransient<ICompanyApplication, CompanyApplication>();

            services.AddTransient<IPictureRepository, PictureRepository>();
            services.AddTransient<IPictureApplication, PictureApplication>();


            services.AddDbContext<GeneralInfoSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<GeneralInfoFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
