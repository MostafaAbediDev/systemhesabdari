using _0_Framework.Application;
using PayrollSystemManagement.Application.Contracts.Department;

namespace PayrollSystemManagement.Application.Contracts.JobTitle
{
    public interface IJobTitleApplication
    {
        OperationResult Create(CreateJobTitle command);
        OperationResult Edit(EditJobTitle command);
        OperationResult Remove(long id);
        OperationResult Restore(long id);
        OperationResult Activate(long id);
        OperationResult Deactivate(long id);
        EditJobTitle GetDetails(long id);
        List<JobTitleViewModel> Search(JobTitleSearchModel searchModel);
        List<JobTitleViewModel> GetJobTitles();
    }
}
