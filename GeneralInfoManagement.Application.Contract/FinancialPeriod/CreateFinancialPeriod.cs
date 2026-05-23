namespace GeneralInfoManagement.Application.Contract.FinancialPeriod
{
    public class CreateFinancialPeriod
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long BranchId { get; set; }
    }
}
