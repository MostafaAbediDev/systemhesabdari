using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.Department;

namespace PayrollSystemManagement.Domain.Payroll.DepartmentAgg
{
    public interface IDepartmentRepository : IRepository<long, Departments>
    {
        EditDepartment GetDetails(long id);
        List<DepartmentViewModel> Search(DepartmentSearchModel searchModel);
        List<DepartmentViewModel> GetDepartments();
    }
}
