namespace PayrollSystemManagement.Application.Contracts.JobTitle
{
    public class CreateJobTitle
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public long DepartmentId { get; set; }
    }
}
