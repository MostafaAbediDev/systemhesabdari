using _0_Framework.Application;
using GeneralInfoManagement.Application.Contract.Company;

namespace GeneralInfoManagement.Application.Contract.FinancialPeriod
{
    public interface IFinancialPeriodApplication
    {
        OperationResult Create(CreateFinancialPeriod command);
        OperationResult Edit(EditFinancialPeriod command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditFinancialPeriod GetDetails(long id);
        List<FinancialPeriodViewModel> Search(FinancialPeriodSearchModel searchModel);
        List<FinancialPeriodViewModel> GetFinancialPeriods();
    }
}
