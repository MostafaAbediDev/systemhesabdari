using _0_Framework.Application;

namespace PayrollSystemManagement.Application.Contracts.Payroll
{

    namespace PayrollSystemManagement.Application.Contracts.Payroll
    {
        public interface IPayrollApplication
        {
            OperationResult Create(CreatePayroll command);
            OperationResult Edit(EditPayroll command);
            OperationResult Approve(long id);
            OperationResult Pay(long id, long accountingDocumentId);
            OperationResult Cancel(long id);
            OperationResult Remove(long id);
            OperationResult Restore(long id);
            OperationResult Activate(long id);
            OperationResult Deactivate(long id);
            EditPayroll GetDetails(long id);
            List<PayrollViewModel> Search(PayrollSearchModel searchModel);
            List<PayrollViewModel> GetAll();
        }
    }
}