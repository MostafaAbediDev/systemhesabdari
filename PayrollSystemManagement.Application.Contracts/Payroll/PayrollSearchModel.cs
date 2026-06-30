namespace PayrollSystemManagement.Application.Contracts.Payroll
{
    public class PayrollSearchModel
    {
        public long? EmployeeId { get; set; }
        public long? BranchId { get; set; }
        public long? FinancialPeriodId { get; set; }
        public int? Status { get; set; }
    }

}