using _0_FrameWork.Domain;
using PayrollSystemManagement.Domain.Payroll.DepartmentAgg;

namespace PayrollSystemManagement.Domain.Payroll.JobTitleAgg
{
    public class JobTitles : EntityBase
    {
        public string Title { get; private set; }
        public string? Description { get; private set; }
        public long DepartmentId { get; private set; }
        public Departments Departments { get; private set; }

        protected JobTitles()
        {
        }

        public JobTitles(string title, string? description, long departmentId)
        {
            Title = title;
            Description = description;
            DepartmentId = departmentId;
        }

        public void Edit(string title, string? description)
        {
            Title = title;
            Description = description;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void DeActivate()
        {
            IsActive = false;
        }
    }
}
