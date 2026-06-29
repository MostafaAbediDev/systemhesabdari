using _0_Framework.Application;

namespace PayrollSystemManagement.Application.Contracts.Department
{
    public interface IDepartmentApplication
    {
        OperationResult Create(CreateDepartment command);
        OperationResult Edit(EditDepartment command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditDepartment GetDetails(long id);
        List<DepartmentViewModel> Search(DepartmentSearchModel searchModel);
        List<DepartmentViewModel> GetDepartments();
    }
}
