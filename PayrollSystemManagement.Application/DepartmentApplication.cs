using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.Department;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;

namespace PayrollSystemManagement.Application
{
    public class DepartmentApplication : IDepartmentApplication
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentApplication(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        public OperationResult Create(CreateDepartment command)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.Name))
                return result.Failed("عنوان دپارتمان الزامی است");

            var department = new Departments(command.Name, command.Description);

            _departmentRepository.Create(department);
            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditDepartment command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            if (string.IsNullOrWhiteSpace(command.Name))
                return result.Failed("عنوان دپارتمان الزامی است");

            var department = _departmentRepository.Get(command.Id);
            if (department == null)
                return result.Failed("دپارتمان یافت نشد");

            department.Edit(command.Name, command.Description);

            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var department = _departmentRepository.Get(id);
            if (department == null)
                return result.Failed("دپارتمان یافت نشد");

            department.Remove();

            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var department = _departmentRepository.Get(id);
            if (department == null)
                return result.Failed("دپارتمان یافت نشد");

            department.Restore();

            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var department = _departmentRepository.Get(id);
            if (department == null)
                return result.Failed("دپارتمان یافت نشد");

            department.Activate();

            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var department = _departmentRepository.Get(id);
            if (department == null)
                return result.Failed("دپارتمان یافت نشد");

            department.Deactivate();

            _departmentRepository.SaveChanges();

            return result.Succedded();
        }

        public EditDepartment GetDetails(long id)
        {

            return _departmentRepository.GetDetails(id);
        }

        public List<DepartmentViewModel> GetDepartments()
        {
            return _departmentRepository.GetDepartments();
        }
        public List<DepartmentViewModel> Search(DepartmentSearchModel searchModel)
        {
            return _departmentRepository.Search(searchModel);
        }
    }
}
