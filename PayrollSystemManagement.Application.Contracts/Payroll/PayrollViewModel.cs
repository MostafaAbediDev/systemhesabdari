namespace PayrollSystemManagement.Application.Contracts.Payroll
{
    public class PayrollViewModel
    {
        public long Id { get; set; }
        public string Employee { get; set; }
        public string Branch { get; set; }
        public string FinancialPeriod { get; set; }
        public string Status { get; set; }
        public decimal TotalBenefits { get; set; } 
        public decimal TotalDeduction { get; set; }
        public decimal NetPay { get; set; }
        public string CreationDate { get; set; }
    }
}
