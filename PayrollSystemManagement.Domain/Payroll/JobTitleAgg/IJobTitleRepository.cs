using _0_FrameWork.Domain;
using PayrollSystemManagement.Application.Contracts.JobTitle;

namespace PayrollSystemManagement.Domain.Payroll.JobTitleAgg
{
    public interface IJobTitleRepository : IRepository<long, JobTitles>
    {
        EditJobTitle GetDetails(long id);
        List<JobTitleViewModel> Search(JobTitleSearchModel searchModel);
        List<JobTitleViewModel> GetJobTitles();
    }
}
