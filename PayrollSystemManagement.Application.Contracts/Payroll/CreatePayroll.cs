namespace PayrollSystemManagement.Application.Contracts.Payroll
{
    public class CreatePayroll
    {
        public long EmployeeId { get; set; }
        public long BranchId { get; set; }
        public long FinancialPeriodId { get; set; }
    }

}