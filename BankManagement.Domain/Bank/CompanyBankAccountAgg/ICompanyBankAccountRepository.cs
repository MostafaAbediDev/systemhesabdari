using _0_FrameWork.Domain;
using BankManagement.Application.Contracts.CompanyBankAccount;

namespace BankManagement.Domain.Bank.CompanyBankAccountAgg
{
    public interface ICompanyBankAccountRepository : IRepository<long, CompanyBankAccounts>
    {
        EditCompanyBankAccount GetDetails(long id);
        List<CompanyBankAccountViewModel> GetCompanyBankAccounts();
        List<CompanyBankAccountViewModel> Search(CompanyBankAccountSearchModel searchModel);
    }
}
