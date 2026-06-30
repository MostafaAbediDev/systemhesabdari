namespace PayrollSystemManagement.Application.Contracts.PayrollDetail
{
    public class PayrollDetailViewModel
    {
        public long Id { get; set; }
        public decimal Quantity { get; set; }
        public decimal Rate { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string PayrollTitle { get; set; }
        public string CreationDate { get; set; }
    }
}
