using _0_Framework.Application;

namespace BankManagement.Application.Contracts.CompanyBankAccount
{
    public interface ICompanyBankAccountApplication
    {
        OperationResult Create(CreateCompanyBankAccount command);
        OperationResult Edit(EditCompanyBankAccount command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditCompanyBankAccount GetDetails(long id);
        List<CompanyBankAccountViewModel> GetCompanyBankAccounts();
        List<CompanyBankAccountViewModel> Search(CompanyBankAccountSearchModel searchModel);
    }
}
