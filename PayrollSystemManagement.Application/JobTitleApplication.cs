using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.JobTitle;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;

namespace PayrollSystemManagement.Application
{
    public class JobTitleApplication : IJobTitleApplication
    {

        private readonly IJobTitleRepository _jobTitleRepository;

        public JobTitleApplication(IJobTitleRepository jobTitleRepository)
        {
            _jobTitleRepository = jobTitleRepository;
        }

        public OperationResult Create(CreateJobTitle command)
        {
            var result = new OperationResult();

            if (string.IsNullOrWhiteSpace(command.Title))
                return result.Failed("عنوان سمت الزامی است");

            if (command.DepartmentId <= 0)
                return result.Failed("دپارتمان معتبر نیست");

            var jobTitle = new JobTitles(
                command.Title,
                command.Description,
                command.DepartmentId
            );

            _jobTitleRepository.Create(jobTitle);
            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Edit(EditJobTitle command)
        {
            var result = new OperationResult();

            if (command.Id <= 0)
                return result.Failed("شناسه نامعتبر است");

            if (string.IsNullOrWhiteSpace(command.Title))
                return result.Failed("عنوان سمت الزامی است");

            if (command.DepartmentId <= 0)
                return result.Failed("دپارتمان معتبر نیست");

            var jobTitle = _jobTitleRepository.Get(command.Id);
            if (jobTitle == null)
                return result.Failed("سمت یافت نشد");

            jobTitle.Edit(command.Title, command.Description);

            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Remove(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var jobTitle = _jobTitleRepository.Get(id);
            if (jobTitle == null)
                return result.Failed("سمت یافت نشد");

            jobTitle.Remove();

            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Restore(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var jobTitle = _jobTitleRepository.Get(id);
            if (jobTitle == null)
                return result.Failed("سمت یافت نشد");

            jobTitle.Restore();

            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }

        public OperationResult Activate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var jobTitle = _jobTitleRepository.Get(id);
            if (jobTitle == null)
                return result.Failed("سمت یافت نشد");

            jobTitle.Activate();

            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }
        public OperationResult Deactivate(long id)
        {
            var result = new OperationResult();

            if (id <= 0)
                return result.Failed("شناسه نامعتبر است");

            var jobTitle = _jobTitleRepository.Get(id);
            if (jobTitle == null)
                return result.Failed("سمت یافت نشد");

            jobTitle.DeActivate();

            _jobTitleRepository.SaveChanges();

            return result.Succedded();
        }

        public EditJobTitle GetDetails(long id)
        {

            return _jobTitleRepository.GetDetails(id);
        }

        public List<JobTitleViewModel> GetJobTitles()
        {
            return _jobTitleRepository.GetJobTitles();
        }
        public List<JobTitleViewModel> Search(JobTitleSearchModel searchModel)
        {
            return _jobTitleRepository.Search(searchModel);
        }
    }
}
