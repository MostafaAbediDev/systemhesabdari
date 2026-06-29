using BankManagement.Application;
using BankManagement.Application.Contracts.Bank;
using BankManagement.Application.Contracts.BankBranch;
using BankManagement.Application.Contracts.BankType;
using BankManagement.Application.Contracts.Cheque;
using BankManagement.Application.Contracts.ChequeBook;
using BankManagement.Application.Contracts.CompanyBankAccount;
using BankManagement.Domain.Bank.BankAgg;
using BankManagement.Domain.Bank.BankBrancheAgg;
using BankManagement.Domain.Bank.BankTypeAgg;
using BankManagement.Domain.Bank.ChequeAgg;
using BankManagement.Domain.Bank.ChequeBookAgg;
using BankManagement.Domain.Bank.CompanyBankAccountAgg;
using BankManagement.Infrastructure.EFCore;
using BankManagement.Infrastructure.EFCore.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BankManagement.Configuration
{
    public class BankManagementBoostrapper
    {
        public static void Configure(IServiceCollection services, string connectionString)
        {
            services.AddTransient<IBankRepository, BankRepository>();
            services.AddTransient<IBankApplication, BankApplication>();

            services.AddTransient<IBankBranchRepository, BankBranchRepository>();
            services.AddTransient<IBankBranchApplication, BankBranchApplication>();

            services.AddTransient<IBankTypeRepository, BankTypeRepository>();
            services.AddTransient<IBankTypeApplication, BankTypeApplication>();

            services.AddTransient<IChequeRepository, ChequeRepository>();
            services.AddTransient<IChequeApplication, ChequeApplication>();

            services.AddTransient<IChequeBookRepository, ChequeBookRepository>();
            services.AddTransient<IChequeBookApplication, ChequeBookApplication>();

            services.AddTransient<ICompanyBankAccountRepository, CompanyBankAccountRepository>();
            services.AddTransient<ICompanyBankAccountApplication, CompanyBankAccountApplication>();


            services.AddDbContext<BankSystemContext>(x => x.UseSqlServer(connectionString));
            services.AddDbContext<BankFakeDataContext>(x => x.UseSqlServer(connectionString));
        }
    }
}
