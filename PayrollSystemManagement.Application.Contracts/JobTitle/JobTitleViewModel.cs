namespace PayrollSystemManagement.Application.Contracts.JobTitle
{
    public class JobTitleViewModel
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string DepartmentName { get; set; }
        public string CreationDate { get; set; }
        public bool IsActive { get; set; }
    }
}
