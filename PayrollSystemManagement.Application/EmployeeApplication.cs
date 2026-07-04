using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.Employee;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;

namespace PayrollSystemManagement.Application
{
    public class EmployeeApplication : IEmployeeApplication
    {
        private readonly IEmployeeRepository _employeeRepository;

        public EmployeeApplication(IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
        }

        public OperationResult Create(CreateEmployee command)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.EmployeeCode))
                return result.Failed("کد پرسنلی الزامی است");

            if (string.IsNullOrWhiteSpace(command.InsuranceNumber))
                return result.Failed("شماره بیمه الزامی است");

            if (command.BaseSalary < 0)
                return result.Failed("حقوق پایه نمی‌تواند منفی باشد");

            if (command.BranchId <= 0)
                return result.Failed("شعبه معتبر نیست");

            if (command.PersonId <= 0)
                return result.Failed("شخص معتبر نیست");

            if (command.DepartmentId <= 0)
                return result.Failed("دپارتمان معتبر نیست");

            if (command.JobTitleId <= 0)
                return result.Failed("سمت شغلی معتبر نیست");

            var employee = new Employees(
                command.EmployeeCode,
                command.PersonId,
                command.BranchId,
                command.DepartmentId,
                command.JobTitleId,
                command.InsuranceNumber,
                command.HireDate,
                command.BaseSalary,
                command.Description,
                (EmployeeContractType)command.ContractType
            );

            _employeeRepository.Create(employee);
            _employeeRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditEmployee command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            if (string.IsNullOrWhiteSpace(command.EmployeeCode))
                return result.Failed("کد پرسنلی الزامی است");

            if (string.IsNullOrWhiteSpace(command.InsuranceNumber))
                return result.Failed("شماره بیمه الزامی است");

            var employee = _employeeRepository.Get(command.Id);
            if (employee == null)
                return result.Failed("کارمند یافت نشد");

            employee.Edit(
                command.EmployeeCode,
                command.DepartmentId,
                command.JobTitleId,
                command.InsuranceNumber,
                command.Description,
                command.BaseSalary
            );

            _employeeRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var employee = _employeeRepository.Get(id);
            if (employee == null)
                return result.Failed("کارمند یافت نشد");

            employee.Remove();

            _employeeRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var employee = _employeeRepository.Get(id);
            if (employee == null)
                return result.Failed("کارمند یافت نشد");

            employee.Restore();

            _employeeRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var employee = _employeeRepository.Get(id);
            if (employee == null)
                return result.Failed("کارمند یافت نشد");

            employee.Activate();

            _employeeRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var employee = _employeeRepository.Get(id);
            if (employee == null)
                return result.Failed("کارمند یافت نشد");

            employee.Deactivate();

            _employeeRepository.SaveChanges();

            return result.Succedded();
        }
        public EditEmployee GetDetails(long id)
        {
            return _employeeRepository.GetDetails(id);
        }

        public List<EmployeeViewModel> GetEmployees()
        {
            return _employeeRepository.GetEmployees();
        }

        public List<EmployeeViewModel> Search(EmployeeSearchModel searchModel)
        {
            return _employeeRepository.Search(searchModel);
        }
    }
}
