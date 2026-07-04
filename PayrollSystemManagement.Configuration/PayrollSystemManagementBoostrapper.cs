using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PayrollSystemManagement.Application;
using PayrollSystemManagement.Application.Contracts.Department;
using PayrollSystemManagement.Application.Contracts.Employee;
using PayrollSystemManagement.Application.Contracts.JobTitle;
using PayrollSystemManagement.Application.Contracts.Payroll.PayrollSystemManagement.Application.Contracts.Payroll;
using PayrollSystemManagement.Application.Contracts.PayrollDetail;
using PayrollSystemManagement.Application.Contracts.PayrollItem;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollDetailAgg;
using PayrollSystemManagement.Domain.Payroll.PayrollItemAgg;
using PayrollSystemManagement.Infrastructure.EFCore;
using PayrollSystemManagement.Infrastructure.EFCore.Repository;

namespace PayrollSystemManagement.Configuration
{
    public class PayrollSystemManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IDepartmentApplication, DepartmentApplication>();

            services.AddTransient<IEmployeeRepository, EmployeeRepository>();
            services.AddTransient<IEmployeeApplication, EmployeeApplication>();

            services.AddTransient<IJobTitleRepository, JobTitleRepository>();
            services.AddTransient<IJobTitleApplication, JobTitleApplication>();

            services.AddTransient<IPayrollRepository, PayrollRepository>();
            services.AddTransient<IPayrollApplication, PayrollApplication>();

            services.AddTransient<IPayrollItemRepository, PayrollItemRepository>();
            services.AddTransient<IPayrollItemApplication, PayrollItemApplication>();


            services.AddTransient<IPayrollDetailRepository, PayrollDetailRepository>();
            services.AddTransient<IPayrollDetailApplication, PayrollDetailApplication>();

            services.AddDbContext<PayrollSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<PayrollFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
