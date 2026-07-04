using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.Department;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class DepatrmentRepository : RepositoryBase<long, Departments>, IDepartmentRepository
    {
        private readonly PayrollFakeDataContext _context;

        public DepatrmentRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }

        public List<DepartmentViewModel> GetDepartments()
        {
            return _context.Departments
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new DepartmentViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public EditDepartment GetDetails(long id)
        {
            return _context.Departments
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditDepartment
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description
                })
                .FirstOrDefault();
        }

        public List<DepartmentViewModel> Search(DepartmentSearchModel searchModel)
        {
            var query = _context.Departments
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchModel.Name))
            {
                var title = searchModel.Name.Trim();
                query = query.Where(x => x.Name.Contains(title));
            }

            var result = query
                .OrderByDescending(x => x.Id)
                .Select(x => new DepartmentViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();

            return result;
        }
    }
}
