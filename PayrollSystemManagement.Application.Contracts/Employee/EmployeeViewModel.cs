namespace PayrollSystemManagement.Application.Contracts.Employee
{
    public class EmployeeViewModel
    {
        public long Id { get; set; }
        public string EmployeeCode { get; set; }
        public string InsuranceNumber { get; set; }
        public string? Description { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public decimal BaseSalary { get; set; }
        public string BranchTitle { get; set; }
        public string PersonName { get; set; }
        public string DepartmentTitle { get; set; }
        public string JobTitle { get; set; }
        public string ContractType { get; set; }
        public string CreationDate { get; set; }
    }
}
