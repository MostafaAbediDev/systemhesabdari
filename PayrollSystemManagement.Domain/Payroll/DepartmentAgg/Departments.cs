using _0_FrameWork.Domain;
using PayrollSystemManagement.Domain.Payroll.JobTitleAgg;

namespace PayrollSystemManagement.Domain.Payroll.DepartmentAgg
{
    public class Departments : EntityBase
    {
        public string Name { get; private set; }
        public string? Description { get; private set; }
        public List<JobTitles> JobTitles { get; private set; }

        protected Departments()
        {
            JobTitles = new List<JobTitles>();
        }

        public Departments(string name, string? description)
        {
            Name = name;
            Description = description;
            IsActive = true;
        }

        public void Edit(string name, string? description)
        {
            Name = name;
            Description = description;
        }

        public void Activate()
        {
            IsActive = true;
        }

        public void Deactivate()
        {
            IsActive = false;
        }

        public void Remove()
        {
            IsDeleted = true;
        }

        public void Restore()
        {
            IsDeleted = false;
        }
    }
}
