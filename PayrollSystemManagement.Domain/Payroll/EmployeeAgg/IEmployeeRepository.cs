using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.Employee;

namespace PayrollSystemManagement.Domain.Payroll.EmployeeAgg
{
    public interface IEmployeeRepository : IRepository<long, Employees>
    {
        EditEmployee GetDetails(long id);
        List<EmployeeViewModel> Search(EmployeeSearchModel searchModel);
        List<EmployeeViewModel> GetEmployees();
    }
}
