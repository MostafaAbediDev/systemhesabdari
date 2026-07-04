using _0_FrameWork.Infrastructure;
using Microsoft.EntityFrameworkCore;
using PayrollSystemManagement.Application.Contracts.JobTitle;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;

namespace PayrollSystemManagement.Infrastructure.EFCore.Repository
{
    public class JobTitleRepository : RepositoryBase<long, JobTitles>, IJobTitleRepository
    {
        private readonly PayrollFakeDataContext _context;

        public JobTitleRepository(PayrollFakeDataContext context) : base(context)
        {
            _context = context;
        }
        public EditJobTitle GetDetails(long id)
        {
            return _context.JobTitles
                .AsNoTracking()
                .Where(x => x.Id == id && !x.IsDeleted)
                .Select(x => new EditJobTitle
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DepartmentId = x.DepartmentId,
                })
                .FirstOrDefault();
        }

        public List<JobTitleViewModel> GetJobTitles()
        {
            return _context.JobTitles
                .AsNoTracking()
                .Where(x => !x.IsDeleted)
                .OrderByDescending(x => x.Id)
                .Select(x => new JobTitleViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DepartmentName = x.Departments.Name,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();
        }

        public List<JobTitleViewModel> Search(JobTitleSearchModel searchModel)
        {
            var query = _context.JobTitles
                .AsNoTracking()
                .Where(x => !x.IsDeleted);

            if (!string.IsNullOrWhiteSpace(searchModel.Title))
            {
                var title = searchModel.Title.Trim();
                query = query.Where(x => x.Title.Contains(title));
            }

            if (!string.IsNullOrWhiteSpace(searchModel.DepartmentName))
            {
                var departmentName = searchModel.DepartmentName.Trim();
                query = query.Where(x => x.Departments.Name.Contains(departmentName));
            }


            var result = query
                .OrderByDescending(x => x.Id)
                .Select(x => new JobTitleViewModel
                {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    DepartmentName = x.Departments.Name,
                    IsActive = x.IsActive,
                    CreationDate = x.CreationDate.ToString("yyyy/MM/dd")
                })
                .ToList();

            return result;
        }
    }
}
