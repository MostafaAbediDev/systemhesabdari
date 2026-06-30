using _0_Framework.Application;

namespace PayrollSystemManagement.Application.Contracts.Employee
{
    public interface IEmployeeApplication
    {
        OperationResult Create(CreateEmployee command);
        OperationResult Edit(EditEmployee command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditEmployee GetDetails(long id);
        List<EmployeeViewModel> Search(EmployeeSearchModel searchModel);
        List<EmployeeViewModel> GetEmployees();
    }
}
