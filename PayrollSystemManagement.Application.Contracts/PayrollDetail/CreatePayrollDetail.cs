namespace PayrollSystemManagement.Application.Contracts.PayrollDetail
{
    public class CreatePayrollDetail
    {
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public long PayrollId { get; set; }
        public long PayrollItemId { get; set; }
    }
}
