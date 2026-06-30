using _0_Framework.Application;

namespace PayrollSystemManagement.Application.Contracts.PayrollDetail
{
    public interface IPayrollDetailApplication
    {
        OperationResult Create(CreatePayrollDetail command);
        OperationResult Edit(EditPayrollDetails command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditPayrollDetails GetDetails(long id);
        List<PayrollDetailViewModel> Search(PayrollDetailsSearchModel searchModel);
        List<PayrollDetailViewModel> GetPayrollDetails();
    }
}
