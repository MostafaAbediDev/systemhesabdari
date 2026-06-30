using _0_Framework.Application;

namespace PayrollSystemManagement.Application.Contracts.PayrollItem
{
    public interface IPayrollItemApplication
    {
        OperationResult Create(CreatePayrollItem command);
        OperationResult Edit(EditPayrollItem command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        OperationResult SetFixed(long id);
        OperationResult UnSetFixed(long id);
        EditPayrollItem GetDetails(long id);
        List<PayrollItemViewModel> Search(PayrollItemSearchModel searchModel);
        List<PayrollItemViewModel> GetPayrollDetails();
    }
}
