namespace PayrollSystemManagement.Application.Contracts.Employee
{
    public class CreateEmployee
    {
        public string EmployeeCode { get; set; }
        public string InsuranceNumber { get; set; }
        public string? Description { get; set; }
        public DateTime HireDate { get; set; }
        public DateTime? TerminationDate { get; set; }
        public decimal BaseSalary { get; set; }
        public long BranchId { get; set; }
        public long PersonId { get; set; }
        public long DepartmentId { get; set; }
        public long JobTitleId { get; set; }
        public EmployeeContractTypeDTO ContractType { get; set; }
    }
}
