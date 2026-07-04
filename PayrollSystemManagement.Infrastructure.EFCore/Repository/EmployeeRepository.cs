using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.Employee;
using PayrollSystemManagement.Domain.Payroll.EmployeeAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class EmployeeRepository : RepositoryBase<long, Employees>, IEmployeeRepository
    {
        private readonly PayrollFakeDataContext _context;

        public EmployeeRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public EditEmployee GetDetails(long id)
        {
            return _context.Employees
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditEmployee
                {
                    Id = x.Id,
                    EmployeeCode = x.EmployeeCode,
                    InsuranceNumber = x.InsuranceNumber,
                    Description = x.Description,
                    HireDate = x.HireDate,
                    TerminationDate = x.TerminationDate,
                    BaseSalary = x.BaseSalary,
                    BranchId = x.BranchId,
                    PersonId = x.PersonId,
                    DepartmentId = x.DepartmentId,
                    JobTitleId = x.JobTitleId,
                    ContractType = (EmployeeContractTypeDTO)x.ContractType
                })
                .FirstOrDefault();
        }

        public List<EmployeeViewModel> GetEmployees()
        {
            return _context.Employees
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new EmployeeViewModel
                {
                    Id = x.Id,
                    EmployeeCode = x.EmployeeCode,
                    InsuranceNumber = x.InsuranceNumber,
                    Description = x.Description,
                    HireDate = x.HireDate,
                    TerminationDate = x.TerminationDate,
                    BaseSalary = x.BaseSalary,
                    BranchTitle = x.Branches.Title,
                    PersonName = x.Persons.FirstName + " " + x.Persons.LastName,
                    DepartmentTitle = x.Departments.Name,
                    JobTitle = x.JobTitles.Title,
                    ContractType = x.ContractType.ToString(),
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public List<EmployeeViewModel> Search(EmployeeSearchModel searchModel)
        {
            var query = _context.Employees
                .AsNoTracking()
                .Include(x => x.Branches)
                .Include(x => x.Persons)
                .Include(x => x.Departments)
                .Include(x => x.JobTitles)
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchModel.EmployeeCode))
            {
                var code = searchModel.EmployeeCode.Trim();
                query = query.Where(x => x.EmployeeCode.Contains(code));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.InsuranceNumber))
            {
                var ins = searchModel.InsuranceNumber.Trim();
                query = query.Where(x => x.InsuranceNumber.Contains(ins));
            }

            return query
                .OrderByDescending(x => x.Id)
                .Select(x => new EmployeeViewModel
                {
                    Id = x.Id,

                    EmployeeCode = x.EmployeeCode,
                    InsuranceNumber = x.InsuranceNumber,
                    Description = x.Description,
                    HireDate = x.HireDate,
                    TerminationDate = x.TerminationDate,
                    BaseSalary = x.BaseSalary,
                    BranchTitle = x.Branches.Title,
                    PersonName = x.Persons.FirstName + " " + x.Persons.LastName,
                    DepartmentTitle = x.Departments.Name,
                    JobTitle = x.JobTitles.Title,
                    ContractType = x.ContractType.ToString(),
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }
    }
}
